using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.Entities;

public sealed class Organizer : Entity<OrganizerId>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }

    private Organizer() { }

    // TODO Create() =>
    public Organizer(OrganizerId id) : base(id) { }
}
