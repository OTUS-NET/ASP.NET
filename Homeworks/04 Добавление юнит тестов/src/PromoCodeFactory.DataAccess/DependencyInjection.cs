using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.DataAccess;

public static class DependencyInjection
{
    public static void AddInMemoryDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<IRepository<Employee>>(_ =>
            new InMemoryRepository<Employee>(SeedData.Employees));
        services.AddSingleton<IRepository<Preference>>(_ =>
            new InMemoryRepository<Preference>(SeedData.Preferences));
        services.AddSingleton<IRepository<Role>>(_ =>
            new InMemoryRepository<Role>(SeedData.Roles));
        services.AddSingleton<IRepository<Customer>>(_ =>
            new InMemoryRepository<Customer>(SeedData.Customers));
        services.AddSingleton<IRepository<Partner>>(_ =>
            new InMemoryRepository<Partner>(SeedData.Partners));
        services.AddSingleton<IRepository<PromoCode>>(_ =>
            new InMemoryRepository<PromoCode>(SeedData.PromoCodes));
        services.AddSingleton<IRepository<CustomerPromoCode>>(_ =>
            new InMemoryRepository<CustomerPromoCode>(SeedData.CustomerPromoCodes));
    }

    public static void AddEfDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<PromoCodeFactoryDbContext>(builder =>
                builder.UseSqlite("Filename=PromoCodeFactory.sqlite"));

        services.AddScoped<IRepository<Employee>, EmployeeEfRepository>();
        services.AddScoped<IRepository<Role>, EfRepository<Role>>();
        services.AddScoped<IRepository<Customer>, CustomerEfRepository>();
        services.AddScoped<IRepository<Partner>, EfRepository<Partner>>();
        services.AddScoped<IRepository<PromoCode>, PromoCodeEfRepository>();
        services.AddScoped<IRepository<Preference>, EfRepository<Preference>>();
        services.AddScoped<IRepository<CustomerPromoCode>, EfRepository<CustomerPromoCode>>();
    }
}
