using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.DataAccess;

public static class DependencyInjection
{
    public static void AddEfDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<PromoCodeFactoryDbContext>((sp, builder) =>
        {
            var conncetionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("PromocodeFactoryDb")
                ?? "Filename=PromoCodeFactory.sqlite";
            builder.UseSqlite(conncetionString);
        });

        services.AddScoped<IRepository<Employee>, EmployeeEfRepository>();
        services.AddScoped<IRepository<Role>, EfRepository<Role>>();
        services.AddScoped<IRepository<Customer>, CustomerEfRepository>();
        services.AddScoped<IRepository<Partner>, PartnerEfRepository>();
        services.AddScoped<IRepository<PartnerPromoCodeLimit>, EfRepository<PartnerPromoCodeLimit>>();
        services.AddScoped<IRepository<PromoCode>, PromoCodeEfRepository>();
        services.AddScoped<IRepository<Preference>, EfRepository<Preference>>();
        services.AddScoped<IRepository<CustomerPromoCode>, EfRepository<CustomerPromoCode>>();
    }
}
