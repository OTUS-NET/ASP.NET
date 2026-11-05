using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Core.Domain;

namespace Pcf.Common.Ef;

public static class HostExtensions
{
    public static IHost MigrateDatabase<TDbContext>(this IHost host) where TDbContext : DbContext
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
            ? $"There are pending migrations '{string.Join(", ", pendingMigrations)}'"
            : "No pending migrations";

        logger.LogInformation(message);

        appContext.Database.Migrate();
        return host;
    }

    public static async Task SeedEntity<T>(
        this IServiceProvider serviceProvider,
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
