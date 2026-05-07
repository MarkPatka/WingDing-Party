using Swashbuckle.AspNetCore.Annotations;

namespace UserService.Api.Models.Request;

public record CreateAvatarForm(
    IFormFile Avatar,
    Guid UserId,
    bool IsDefault,
    bool IsActive)
{
    internal Stream AvatarStream;

    [SwaggerIgnore] public string FileName { get; set; }

    [SwaggerIgnore] public string ContentType { get; set; }
}