using ClubService.Domain.ClubAggregate;
using ClubService.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Persistence;

public class UserServiceDbContext(DbContextOptions<UserServiceDbContext> options)
    : DbContext(options)
{
    public DbSet<Club> Clubs { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UserServiceDbContext).Assembly);
    }
}