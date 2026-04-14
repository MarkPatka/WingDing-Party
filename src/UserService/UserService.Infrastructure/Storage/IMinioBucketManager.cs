namespace UserService.Infrastructure.Storage;

public interface IMinioBucketManager
{
    Task EnsureBucketExistsAsync(string bucket, CancellationToken cancellationToken = default);
}