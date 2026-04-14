using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using UserService.Application.Common.Configuration;

namespace UserService.Infrastructure.Storage;

public class MinioBucketManager : IMinioBucketManager
{
    private readonly IMinioClient _client;

    public MinioBucketManager(IOptions<FileStorageOptions> options)
    {
        _client = new MinioClient()
            .WithEndpoint(options.Value.Endpoint)
            .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
            .WithSSL(options.Value.WithSsl)
            .Build();
    }

    public async Task EnsureBucketExistsAsync(string bucket, CancellationToken cancellationToken = default)
    {
        var exists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucket),
            cancellationToken);

        if (!exists)
        {
            await _client.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucket),
                cancellationToken);
        }
    }
}