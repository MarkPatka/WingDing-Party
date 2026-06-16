namespace AuthService.Infrastructure.Common.Configuration;

/// <summary>
/// That keys are used as prefixes for both: the settings in .env file and appsettings json blocks
/// 'PREFIX__' in .env and "{ "Prefix": {...}}" at json appsettings.
/// </summary>
public static class ConfigurationKeys
{
    public const string KEYCLOAK_SECTION = "Keycloak";
    public const string AUTHENTICATION_SECTION = "Authentication";


}
