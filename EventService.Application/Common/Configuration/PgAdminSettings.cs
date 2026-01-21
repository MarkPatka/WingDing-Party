namespace EventService.Application.Common.Configuration;

public class PgAdminSettings
{
    public const string SectionName = nameof(PgAdminSettings);

    public string DefaultEmail { get; set; } = null!;
    public string DefaultPassword { get; set; } = null!;

    public int Port { get; set; }

}
