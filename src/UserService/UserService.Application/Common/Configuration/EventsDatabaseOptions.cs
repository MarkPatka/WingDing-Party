namespace UserService.Application.Common.Configuration;

public class EventsDatabaseOptions
{
    public const string SectionName = nameof(EventsDatabaseOptions);

    public string POSTGRES_DB { get; set; } = null!;
    public int POSTGRES_PORT { get; set; } = 5432;
    public string POSTGRES_USER { get; set; } = null!;
    public string POSTGRES_PASSWORD { get; set; } = null!;

    public int MINIO_PORT { get; set; } = 9000;
    public string MINIO_USER { get; set; } = null!;
    public string MINIO_PASSWORD { get; set; } = null!;

    public string DB_CONNECTION_STRING { get; set; } = null!;
}