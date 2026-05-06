namespace AuthService.Infrastructure.Common.Configuration;

public sealed class RedisOptions
{
    public string REDIS_HOST { get; set; } = null!;
    public int REDIS_PORT { get; set; } = 6379;
    public string? REDIS_PASSWORD { get; set; }
    public string REDIS_CONNECTION_STRING => string.IsNullOrWhiteSpace(REDIS_PASSWORD)
        ? $"{REDIS_HOST}:{REDIS_PORT}"
        : $"{REDIS_HOST}:{REDIS_PORT},password={REDIS_PASSWORD}";
}
