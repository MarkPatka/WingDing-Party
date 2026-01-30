using EventService.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.Persistence
{
    public class EventServiceDbContext(DbContextOptions<EventServiceDbContext> options)
        : DbContext(options)
    {
        public DbSet<Event> Events { get; set; } = null!;
    }
}
