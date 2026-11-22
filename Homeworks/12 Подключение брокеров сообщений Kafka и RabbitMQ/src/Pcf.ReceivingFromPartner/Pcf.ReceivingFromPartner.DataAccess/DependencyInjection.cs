using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Ef;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.DataAccess.Repositories;
using StackExchange.Redis;

namespace Pcf.ReceivingFromPartner.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddEfDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<PromoCodeFactoryDbContext>((sp, builder) =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("PromocodeFactoryDb")
                ?? throw new InvalidOperationException("Connection string 'PromocodeFactoryDb' is not configured.");
            builder.UseNpgsql(connectionString);
        });

        services.AddScoped<IRepository<Partner>, PartnerEfRepository>();
        services.AddScoped<IRepository<PartnerPromoCodeLimit>, EfRepository<PromoCodeFactoryDbContext, PartnerPromoCodeLimit>>();
        services.AddScoped<IRepository<PromoCode>, PromoCodeEfRepository>();
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");
            return ConnectionMultiplexer.Connect(connectionString);
        });
        services.AddScoped<IRepository<Preference>, PreferenceRedisRepository>();
        return services;
    }
}
