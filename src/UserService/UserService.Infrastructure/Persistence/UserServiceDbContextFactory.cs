using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Persistence;

public class UserServiceDbContextFactory : IDesignTimeDbContextFactory<UserServiceDbContext>
{
    public UserServiceDbContext CreateDbContext(string[] args)
    {
        // 🔧 Robust path resolution: find .env relative to solution root
        var currentDir = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDir);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(solutionRoot)
            .AddJsonFile("src/UserService/UserService.Api/appsettings.json", optional: true)
            .AddJsonFile("src/UserService/UserService.Api/appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Load .env from solution root
        var envPath = Path.Combine(solutionRoot, ".env");
        if (File.Exists(envPath))
        {
            DotNetEnv.Env.Load(envPath);
        }

        // ✅ Connection string: prioritize env var (works in Docker & local)
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? configuration["CONNECTION_STRING"]
            ?? "Host=localhost;Port=5432;Database=userdb;Username=postgres;Password=postgres";

        // ✅ Match runtime Npgsql config
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var optionsBuilder = new DbContextOptionsBuilder<UserServiceDbContext>();
        optionsBuilder.UseNpgsql(connectionString,
            npgsql => npgsql.EnableRetryOnFailure(2));

        return new UserServiceDbContext(optionsBuilder.Options);
    }

    // 🔍 Helper: walk up directories to find solution root (look for .env or docker-compose.yml)
    private static string FindSolutionRoot(string startPath)
    {
        var current = new DirectoryInfo(startPath);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, ".env")) ||
                File.Exists(Path.Combine(current.FullName, "docker-compose.yml")))
            {
                return current.FullName;
            }
            current = current.Parent;
        }
        // Fallback
        return startPath;
    }
}