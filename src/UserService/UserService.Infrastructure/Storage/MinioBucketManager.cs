using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using UserService.Application.Common.Exceptions;

namespace UserService.Infrastructure.Storage;

public class MinioBucketManager : IMinioBucketManager
{
    private readonly IMinioClient _client;
    private readonly ILogger<MinioBucketManager> _logger;

    public MinioBucketManager(IMinioClient client, ILogger<MinioBucketManager> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task EnsurePathExistsAsync(string path, CancellationToken ct = default)
    {
        var parts = path.Split('/');
        var bucket = parts[0];
        var folder = string.Join('/', parts.Skip(1));

        try
        {
            var isExistBucket = await _client.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucket),
                ct);

            if (!isExistBucket)
            {
                await _client.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucket),
                    ct);
            }
        }
        catch (Exception e)
        {
            var mes = $"Exception occured while creating bucket {bucket} due to {e.Message}";
            _logger.LogError(mes);
            throw new FileStorageException(mes);
        }

        if (string.IsNullOrEmpty(folder))
        {
            return;
        }

        try
        {
            var isExistFolder = await _client.ListObjectsEnumAsync(
                new ListObjectsArgs()
                    .WithBucket(bucket)
                    .WithPrefix(folder)
                    .WithRecursive(true), ct)
                .AnyAsync();
            
            if (isExistFolder)
            {
                return;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Exception occured while verifying folder {folder} in bucket {bucket} due to {message}",
                folder, bucket, e.Message);

            throw new FileStorageException($"Exception occured while verifying folder {folder} in bucket {bucket} due to {e.Message}");
        }

        try
        {
            await _client.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(folder)
                    .WithStreamData(new MemoryStream(Array.Empty<byte>()))
                    .WithObjectSize(0)
                    .WithContentType("application/octet-stream")
            );
        }
        catch (Exception e)
        {
            var mes = $"Exception occured while create folder {folder} in bucket {bucket} due to {e.Message}";
            _logger.LogError(mes);
            throw new FileStorageException(mes);
        }
    }

    public async Task MakeBucketPublicAsync(string bucket, CancellationToken ct = default)
    {
        try
        {
            var policy = $@"
            {{
              ""Version"": ""2012-10-17"",
              ""Statement"": [
                {{
                  ""Effect"": ""Allow"",
                  ""Principal"": ""*"",
                  ""Action"": [""s3:GetObject""],
                  ""Resource"": [""arn:aws:s3:::{bucket}/*""]
                }}
              ]
            }}";

            await _client.SetPolicyAsync(
                new SetPolicyArgs()
                    .WithBucket(bucket)
                    .WithPolicy(policy),
                ct);
        }
        catch (Exception e)
        {
            var mes = $"Exception occured while create folder {bucket} in bucket {bucket} due to {e.Message}";
            _logger.LogError(mes);
            throw new FileStorageException(mes);
        }
    }
}