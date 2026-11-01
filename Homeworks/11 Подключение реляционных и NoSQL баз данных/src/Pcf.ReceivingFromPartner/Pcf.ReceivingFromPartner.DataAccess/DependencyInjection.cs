using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Ef;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.DataAccess.Repositories;

namespace Pcf.ReceivingFromPartner.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddEfDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<PromoCodeFactoryDbContext>((sp, builder) =>
        {
            var conncetionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("PromocodeFactoryDb")
                ?? throw new InvalidOperationException("Connection string 'PromocodeFactoryDb' is not configured.");
            builder.UseSqlite(conncetionString);
        });

        services.AddScoped<IRepository<Partner>, PartnerEfRepository>();
        services.AddScoped<IRepository<PartnerPromoCodeLimit>, EfRepository<PromoCodeFactoryDbContext, PartnerPromoCodeLimit>>();
        services.AddScoped<IRepository<PromoCode>, PromoCodeEfRepository>();
        services.AddScoped<IRepository<Preference>, EfRepository<PromoCodeFactoryDbContext, Preference>>();
        return services;
    }
}
