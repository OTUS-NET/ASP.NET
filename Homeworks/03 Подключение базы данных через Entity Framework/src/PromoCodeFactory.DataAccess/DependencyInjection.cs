using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.DataAccess;

public static class DependencyInjection
{
    public static void AddInMemoryDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<IRepository<Employee>>((x) =>
            new InMemoryRepository<Employee>(FakeDataFactory.Employees));
        services.AddSingleton<IRepository<Preference>>((x) =>
            new InMemoryRepository<Preference>([]));
        services.AddSingleton<IRepository<Role>>((x) =>
            new InMemoryRepository<Role>(FakeDataFactory.Roles));
        services.AddSingleton<IRepository<Customer>>((x) =>
            new InMemoryRepository<Customer>(FakeDataFactory.Customers));
        services.AddSingleton<IRepository<PromoCode>>((x) =>
            new InMemoryRepository<PromoCode>(FakeDataFactory.PromoCodes));
        services.AddSingleton<IRepository<CustomerPromoCode>>((x) =>
            new InMemoryRepository<CustomerPromoCode>([]));
    }

    public static void AddEfDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<PromoCodeFactoryDbContext>(builder =>
                builder.UseSqlite("Filename=PromoCodeFactory.sqlite"));

        services.AddScoped<IRepository<Employee>, EfRepository<Employee>>();
        services.AddScoped<IRepository<Role>, EfRepository<Role>>();
        services.AddScoped<IRepository<Customer>, EfRepository<Customer>>();
        services.AddScoped<IRepository<PromoCode>, EfRepository<PromoCode>>();
        services.AddScoped<IRepository<Preference>, EfRepository<Preference>>();
        services.AddScoped<IRepository<CustomerPromoCode>, EfRepository<CustomerPromoCode>>();
    }
}
