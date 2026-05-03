namespace ClubService.Application.Common.Configuration;

public class ClubsDatabaseOptions
{
    public const string SectionName = nameof(ClubsDatabaseOptions);

    public string POSTGRES_DB { get; set; } = null!;
    public int POSTGRES_PORT { get; set; } = 5432;
    public string POSTGRES_USER { get; set; } = null!;
    public string POSTGRES_PASSWORD { get; set; } = null!;
    public string CONNECTION_STRING { get; set; } = null!;
}