using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.DataAccess;

public static class DependencyInjection
{
    public static void AddDataAccess(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IRepository<Employee>), (x) =>
            new InMemoryRepository<Employee>(FakeDataFactory.Employees));
        services.AddSingleton(typeof(IRepository<Role>), (x) =>
            new InMemoryRepository<Role>(FakeDataFactory.Roles));
        services.AddSingleton(typeof(IRepository<Customer>), (x) =>
            new InMemoryRepository<Customer>(FakeDataFactory.Customers));
        services.AddSingleton(typeof(IRepository<PromoCode>), (x) =>
            new InMemoryRepository<PromoCode>(FakeDataFactory.PromoCodes));
    }
}
