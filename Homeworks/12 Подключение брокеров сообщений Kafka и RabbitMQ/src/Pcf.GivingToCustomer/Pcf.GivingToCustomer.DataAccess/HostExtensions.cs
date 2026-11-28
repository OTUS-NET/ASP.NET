using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pcf.Common.Ef;

namespace Pcf.GivingToCustomer.DataAccess;

public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PromoCodeFactoryDbContext>();
        context.Database.EnsureCreated();
        return host;
    }

    public static async Task SeedDatabase(this IHost host, CancellationToken ct = default)
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("SeedDatabase");

        logger.LogInformation("Starting database seed...");

        await scope.ServiceProvider.SeedEntity(logger, SeedData.Preferences, ct);
        await scope.ServiceProvider.SeedEntity(logger, SeedData.Customers, ct);
        await scope.ServiceProvider.SeedEntity(logger, SeedData.PromoCodes, ct);

        logger.LogInformation("Database seed completed.");
    }
}
