namespace AuthService.Infrastructure.Common.Configuration;

public sealed class AuthDatabaseOptions
{
    public string POSTGRES_DB { get; set; } = null!;
    public int POSTGRES_PORT { get; set; } = 5432;
    public string POSTGRES_USER { get; set; } = null!;
    public string POSTGRES_PASSWORD { get; set; } = null!;
    public string CONNECTION_STRING { get; set; } = null!;
}