namespace UserService.Infrastructure.Storage;

public interface IMinioBucketManager
{
    Task EnsurePathExistsAsync(string path, CancellationToken ct = default);
    Task MakeBucketPublicAsync(string bucket, CancellationToken ct = default);
}