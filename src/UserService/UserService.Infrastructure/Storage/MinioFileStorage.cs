using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using UserService.Application.Common.Configuration;
using UserService.Application.Common.Exceptions;
using UserService.Application.Services;

namespace UserService.Infrastructure.Storage;

public sealed class MinioFileStorage : IFileStorage
{
    private readonly IMinioClient _client;
    private readonly FileStorageOptions _options;
    private readonly IMinioBucketManager _bucketManager;

    public MinioFileStorage(
        IOptions<FileStorageOptions> options,
        IMinioClient client,
        IMinioBucketManager bucketManager)
    {
        _options = options.Value;
        _bucketManager = bucketManager;
        _client = client;
    }

    public async Task<Uri> SaveAsync(
        Stream content,
        string fileName,
        string path,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        await _bucketManager.EnsurePathExistsAsync(path, cancellationToken);

        var parts = path.Split('/');
        var bucket = parts[0];

        await _bucketManager.MakeBucketPublicAsync(bucket, cancellationToken);

        fileName = string.Join('/', parts.Skip(1)) + fileName;

        try
        {
            await _client.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(fileName)
                    .WithStreamData(content)
                    .WithObjectSize(content.Length)
                    .WithContentType(contentType),
                cancellationToken);
        }
        catch (Exception e)
        {
            throw new FileStorageException(
                $"Exception occured while uploading file {fileName} due to {e.Message}");
        }

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