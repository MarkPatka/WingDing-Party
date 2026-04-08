using EventService.Domain;
using EventService.Domain.Common.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EventService.Infrastructure.Persistence
{
    public class EventServiceDbContext(DbContextOptions<EventServiceDbContext> options)
        : DbContext(options)
    {
        public DbSet<Event> Events { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(EventServiceDbContext).Assembly);

            modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p => p.IsPrimaryKey())
                .ToList()
                .ForEach(e => e.ValueGenerated = ValueGenerated.Never);

            modelBuilder.Ignore<IDomainEvent>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
