namespace UserService.Application.Common.Configuration;

public class KafkaOptions
{
    public const string SectionName = "KafkaOptions";
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string ProduceEventsTopic { get; set; } = "user-service-events";
    public string DeadLetterTopic { get; set; } = "user-service-events-dlq";
    public KafkaAcks Acks { get; set; } = KafkaAcks.All;
    public bool EnableIdempotence { get; set; } = true;

    public string ConsumerGroupId { get; set; } = "user-service-consumer";
    public string[] ConsumeEventsTopics { get; set; } = [];
    public string AutoOffsetReset { get; set; } = "Earliest";

    public int MaxRetryAttempts { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
    public int MessageTimeoutMs { get; set; } = 5000;
}