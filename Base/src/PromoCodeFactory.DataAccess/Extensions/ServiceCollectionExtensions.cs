using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.DataAccess.Seeding;

namespace PromoCodeFactory.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPromoCodesDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PromoCodesDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.UseSnakeCaseNamingConvention();
            options.LogTo(Console.WriteLine, LogLevel.Information);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PromoCodesDbContext>();

        // dbContext.Database.EnsureDeleted();
        
        dbContext.Database.Migrate();
        
        return services;
    }
}