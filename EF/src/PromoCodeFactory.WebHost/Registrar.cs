using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using System;

namespace PromoCodeFactory.WebHost
{
    public static class Registrar
    {
        public static IServiceCollection AddRepository(this IServiceCollection services) 
        {
            //services.AddSingleton(typeof(IRepository<Employee, Guid>), (x) => new InMemoryRepository<Employee, Guid>(FakeDataFactory.Employees));
            //services.AddSingleton(typeof(IRepository<Role,Guid>), (x) => new InMemoryRepository<Role,Guid>(FakeDataFactory.Roles));
            services.AddScoped<IRepository<Employee, Guid>, EFRepository<Employee, Guid>>();
            services.AddScoped<IRepository<Role, Guid>, EFRepository<Role, Guid>>();
            services.AddScoped<ICostomerRepository, CostomerRepository>();
            services.AddScoped<IRepository<Preference, Guid>, EFRepository<Preference, Guid>>();
            services.AddScoped<IRepository<PromoCode, Guid>, EFRepository<PromoCode, Guid>>();
            services.AddScoped<IRepository<CustomerPreference, Guid>, EFRepository<CustomerPreference, Guid>>();

            return services;
        }
    }
}
