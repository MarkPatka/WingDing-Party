namespace EventService.Application.Common.Configuration;

public class ApiSettings
{
    public const string SectionName = nameof(ApiSettings);

    public string[] CorsOrigins { get; set; } = [];
    public int Port { get; set; }
}
