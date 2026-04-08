namespace UserService.Application.Common.Configuration;

public class ApiOptions 
{
    public const string SectionName = nameof(ApiOptions);

    public string[] CorsOrigins { get; set; } = [];
    public int Port { get; set; }
}
