using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Ef;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Repositories;

namespace Pcf.GivingToCustomer.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddEfDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<PromoCodeFactoryDbContext>((sp, builder) =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("PromocodeFactoryDb")
                ?? throw new InvalidOperationException("Connection string 'PromocodeFactoryDb' is not configured.");
            builder.UseSqlite(connectionString);
        });

        services.AddScoped<IRepository<Customer>, CustomerEfRepository>();
        services.AddScoped<IRepository<Preference>, EfRepository<PromoCodeFactoryDbContext, Preference>>();
        services.AddScoped<IRepository<PromoCode>, PromoCodeEfRepository>();
        services.AddScoped<IRepository<CustomerPromoCode>, EfRepository<PromoCodeFactoryDbContext, CustomerPromoCode>>();
        return services;
    }
}
