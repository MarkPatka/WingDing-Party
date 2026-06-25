using System.Text.Json.Serialization;

namespace AuthService.Infrastructure.Common.Dto;

internal sealed class CredentialRepresentationModel
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "password";

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    [JsonPropertyName("temporary")]
    public bool Temporary { get; init; }
}
