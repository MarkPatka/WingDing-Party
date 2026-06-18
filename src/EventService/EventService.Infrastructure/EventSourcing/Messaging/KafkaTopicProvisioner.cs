using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EventService.Application.Common.Configuration;
using EventService.Application.EventSourcing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.Messaging;

internal sealed class KafkaTopicProvisioner(
    IOptions<KafkaOptions> options,
    ILogger<KafkaTopicProvisioner> logger) : ITopicProvisioner
{
    private static readonly TimeSpan MetadataTimeout = TimeSpan.FromSeconds(30);

    private readonly KafkaOptions _options = options.Value;
    private readonly ILogger<KafkaTopicProvisioner> _logger = logger;

    public async Task ProvisionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string[] desiredTopics = CollectDesiredTopics();

        AdminClientConfig config = new() { BootstrapServers = _options.BootstrapServers };
        using IAdminClient adminClient = new AdminClientBuilder(config).Build();

        HashSet<string> existing = GetExistingTopics(adminClient);

        List<TopicSpecification> toCreate = desiredTopics
            .Where(topic => !existing.Contains(topic))
            .Select(topic => new TopicSpecification
            {
                Name = topic,
                NumPartitions = _options.TopicPartitions,
                ReplicationFactor = _options.TopicReplicationFactor
            })
            .ToList();

        if (toCreate.Count == 0)
        {
            _logger.LogInformation(
                "Kafka topics already present: [{Topics}]",
                string.Join(", ", desiredTopics));
            return;
        }

        try
        {
            await adminClient.CreateTopicsAsync(toCreate);

            _logger.LogInformation(
                "Provisioned Kafka topics: [{Topics}] (partitions={Partitions}, rf={Rf})",
                string.Join(", ", toCreate.Select(spec => spec.Name)),
                _options.TopicPartitions,
                _options.TopicReplicationFactor);
        }
        catch (CreateTopicsException ex)
        {
            // Гонка: топик мог быть создан владельцем (другим сервисом) между чтением
            // metadata и CreateTopics — это не ошибка. Любой другой код — фатально.
            foreach (CreateTopicReport report in ex.Results
                .Where(report => report.Error.Code == ErrorCode.TopicAlreadyExists))
            {
                _logger.LogInformation(
                    "Topic '{Topic}' already exists (created concurrently).", report.Topic);
            }

            List<CreateTopicReport> failures = ex.Results
                .Where(report => report.Error.Code != ErrorCode.NoError
                              && report.Error.Code != ErrorCode.TopicAlreadyExists)
                .ToList();

            if (failures.Count > 0)
            {
                string detail = string.Join(
                    "; ", failures.Select(f => $"{f.Topic}: {f.Error.Reason}"));
                _logger.LogError(ex, "Failed to provision Kafka topics: {Detail}", detail);
                throw;
            }
        }
    }

    private string[] CollectDesiredTopics()
    {
        IEnumerable<string> topics = _options.ConsumeEventsTopics
            .Append(_options.ProduceEventsTopic)
            .Append(_options.DeadLetterTopic);

        return topics
            .Where(topic => !string.IsNullOrWhiteSpace(topic))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private static HashSet<string> GetExistingTopics(IAdminClient adminClient)
    {
        Metadata metadata = adminClient.GetMetadata(MetadataTimeout);

        return metadata.Topics
            .Where(topic => topic.Error.Code == ErrorCode.NoError)
            .Select(topic => topic.Topic)
            .ToHashSet(StringComparer.Ordinal);
    }
}
