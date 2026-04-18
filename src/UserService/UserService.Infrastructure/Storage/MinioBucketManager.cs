using System.Reactive.Linq;
using Microsoft.Extensions.Options;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;
using Minio.Exceptions;
using UserService.Application.Common.Configuration;
using UserService.Application.Common.Exceptions;

namespace UserService.Infrastructure.Storage;

public class MinioBucketManager : IMinioBucketManager
{
    private readonly IMinioClient _client;

    public MinioBucketManager(IMinioClient client)
    {
        _client = client;
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
        catch (BucketNotFoundException)
        {
        }
        catch (Exception e)
        {
            throw new FileStorageException(
                $"Exception occured while creating bucket {bucket} due to {e.Message}");
        }

        if (string.IsNullOrEmpty(folder))
        {
            return;
        }

        try
        {
            var isExistFolder = await _client.ListObjectsAsync(
                new ListObjectsArgs()
                    .WithBucket(bucket)
                    .WithPrefix(folder)
                    .WithRecursive(true)).Any();
            if (isExistFolder)
            {
                return;
            }
        }
        catch (Exception e)
        {
            throw new FileStorageException(
                $"Exception occured while verifying folder {folder} in bucket {bucket} due to {e.Message}");
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
            throw new FileStorageException(
                $"Exception occured while create folder {folder} in bucket {bucket} due to {e.Message}");
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
            throw new FileStorageException(
                $"Exception occured while create folder {bucket} in bucket {bucket} due to {e.Message}");
        }
    }
}