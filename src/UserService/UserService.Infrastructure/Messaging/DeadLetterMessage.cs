namespace UserService.Infrastructure.Messaging;

public class DeadLetterMessage
{
    public Guid OriginalMessageId { get; init; }
    public string OriginalType { get; init; } = default!;
    public string Payload { get; init; } = default!;
    public string Error { get; init; } = default!;
    public string? StackTrace { get; init; }
    public int Retries { get; init; }
    public DateTime FailedAtUtc { get; init; }
}