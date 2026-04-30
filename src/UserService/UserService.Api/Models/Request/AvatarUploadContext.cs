namespace UserService.Api.Models.Request;

public class AvatarUploadContext
{
    public Stream Stream { get; init; }
    public string FileName { get; init; }
    public string ContentType { get; init; }
}