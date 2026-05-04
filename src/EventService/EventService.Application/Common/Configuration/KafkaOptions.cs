namespace EventService.Application.Common.Configuration;

public class KafkaOptions
{
    public const string SectionName = "KafkaOptions";

    public string BootstrapServers { get; set; } = "localhost:9092";
    public string ConsumerGroupId { get; set; } = "event-service-consumer";
    public string ProduceEventsTopic { get; set; } = "event-service-events";
    public string[] ConsumeEventsTopic { get; set; } = ["userprofile-events"];
    public string AutoOffsetReset { get; set; } = "Earliest";
    public string DeadLetterTopic { get; set; } = "event-service-events-dlq";
    public int MaxRetryAttempts { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
}
