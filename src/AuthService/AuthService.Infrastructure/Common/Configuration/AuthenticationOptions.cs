namespace AuthService.Infrastructure.Common.Configuration;

public sealed class AuthenticationOptions
{
    public const string SectionName = nameof(AuthenticationOptions);

    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string MetadataUrl { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; init; }
}