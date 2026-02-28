using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PromoCodeFactory.DataAccess;

public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host) => host.MigrateDatabase<PromoCodeFactoryDbContext>();

    private static IHost MigrateDatabase<TDbContext>(this IHost host) where TDbContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        using var appContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("MigrationManager");

        var connectionString = appContext.Database.GetConnectionString();
        logger.LogInformation($"Use connectionString: '{connectionString}'");

        var pendingMigrations = appContext.Database.GetPendingMigrations().ToArray();
        var message = pendingMigrations.Length > 0
            ? $"There are pending migrations '{string.Join(',', pendingMigrations)}'"
            : "No pending migrations";

        logger.LogInformation(message);

        appContext.Database.Migrate();
        return host;
    }
}
