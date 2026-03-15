using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess;

public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host) => host.MigrateDatabase<PromoCodeFactoryDbContext>();

    public static async Task SeedDatabase(this IHost host, CancellationToken ct = default)
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("SeedDatabase");

        logger.LogInformation("Starting database seed...");

        await SeedEntity(scope.ServiceProvider, logger, SeedData.Roles, ct);
        await SeedEntity(scope.ServiceProvider, logger, SeedData.Preferences, ct);
        await SeedEntity(scope.ServiceProvider, logger, SeedData.Employees, ct);
        await SeedEntity(scope.ServiceProvider, logger, SeedData.Customers, ct);
        await SeedEntity(scope.ServiceProvider, logger, SeedData.Partners, ct);
        await SeedEntity(scope.ServiceProvider, logger, SeedData.PartnerPromoCodeLimits, ct);
        await SeedEntity(scope.ServiceProvider, logger, SeedData.PromoCodes, ct);
        await SeedEntity(scope.ServiceProvider, logger, SeedData.CustomerPromoCodes, ct);

        logger.LogInformation("Database seed completed.");
    }

    private static IHost MigrateDatabase<TDbContext>(this IHost host) where TDbContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        using var appContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("MigrateDatabase");

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

    public static async Task SeedEntity<T>(
        IServiceProvider serviceProvider,
        ILogger logger,
        IReadOnlyCollection<T> entities,
        CancellationToken ct)
        where T : BaseEntity
    {
        var repository = serviceProvider.GetRequiredService<IRepository<T>>();

        if ((await repository.GetAll()).Count > 0)
        {
            logger.LogInformation($"Entity '{typeof(T).Name}' is not empty. Skip seeding");
            return;
        }

        logger.LogInformation($"Starting seed {typeof(T).Name}");

        foreach (var entity in entities)
            await repository.Add(entity, ct);
    }
}
