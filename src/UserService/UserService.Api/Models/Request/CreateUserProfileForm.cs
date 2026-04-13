using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace UserService.Api.Models.Request;

public record CreateUserProfileForm(
    IFormFile? Avatar,
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate)
{
    [SwaggerIgnore]
    public Uri? AvatarUri { get; set; }
}