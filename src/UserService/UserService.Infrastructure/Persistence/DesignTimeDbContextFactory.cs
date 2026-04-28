using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserService.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserServiceDbContext>
{
    public UserServiceDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("CONNECTION_STRING") ??
                   throw new ArgumentException("CONNECTION_STRING environment variable is not set");
        
        var optionsBuilder = new DbContextOptionsBuilder<UserServiceDbContext>();
        optionsBuilder.UseNpgsql(conn, o => o.EnableRetryOnFailure(2));

        return new UserServiceDbContext(optionsBuilder.Options);
    }
}