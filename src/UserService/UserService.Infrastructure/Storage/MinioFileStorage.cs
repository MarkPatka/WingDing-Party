using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using UserService.Application.Common.Configuration;
using UserService.Application.Services;

namespace UserService.Infrastructure.Storage;

public sealed class MinioFileStorage : IFileStorage
{
    private readonly IMinioClient _client;
    private readonly FileStorageOptions _options;
    private readonly IMinioBucketManager _bucketManager;

    public MinioFileStorage(
        IOptions<FileStorageOptions> options,
        IMinioBucketManager bucketManager)
    {
        _options = options.Value;
        _bucketManager = bucketManager;

        _client = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(_options.WithSsl)
            .Build();
    }

    public async Task<Uri> SaveAsync(
        Stream content,
        string fileName,
        string path,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        // path = bucket
        var bucket = path.Trim('/');
        var objectName = $"{bucket}/{fileName}";
        await _bucketManager.EnsureBucketExistsAsync(bucket, cancellationToken);
        
        await _client.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(fileName)
                .WithStreamData(content)
                .WithObjectSize(content.Length)
                .WithContentType(contentType),
            cancellationToken);
        
        var uri = new Uri(
            $"{(_options.WithSsl ? "https" : "http")}://{_options.Endpoint}/{bucket}/{fileName}");

        return uri;
    }

    public async Task DeleteAsync(
        Uri fileUri,
        CancellationToken cancellationToken = default)
    {
        var (bucket, objectName) = Parse(fileUri);

        await _client.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName),
            cancellationToken);
    }

    private static (string Bucket, string ObjectName) Parse(Uri uri)
    {
        var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
        if (segments.Length < 2)
            throw new InvalidOperationException($"Invalid file URL format. {uri}");

        return (segments[0], segments[1]);
    }
}