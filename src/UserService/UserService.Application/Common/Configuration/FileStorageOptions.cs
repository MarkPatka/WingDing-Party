namespace UserService.Application.Common.Configuration;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public string Endpoint { get; init; } = default!;
    public string AccessKey { get; init; } = default!;
    public string SecretKey { get; init; } = default!;
    public string AvatarBucket { get; init; } = default!;
    public bool WithSsl { get; init; }
}