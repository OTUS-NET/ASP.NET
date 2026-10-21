using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pcf.Common.Ef;

namespace Pcf.ReceivingFromPartner.DataAccess;

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

        await scope.ServiceProvider.SeedEntity(logger, SeedData.Preferences, ct);
        await scope.ServiceProvider.SeedEntity(logger, SeedData.Partners, ct);
        await scope.ServiceProvider.SeedEntity(logger, SeedData.PartnerPromoCodeLimits, ct);
        await scope.ServiceProvider.SeedEntity(logger, SeedData.PromoCodes, ct);

        logger.LogInformation("Database seed completed.");
    }
}
