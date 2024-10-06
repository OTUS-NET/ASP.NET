using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.EntityFramework;
using PromoCodeFactory.WebHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Compose
{
    public static class Configuration
    {
        public static IServiceCollection GetServiceCollection(this IConfigurationRoot configuration, IServiceCollection serviceCollection = null)
        {
            if (serviceCollection == null)
            {
                serviceCollection = new ServiceCollection();
            }
            serviceCollection
                .AddSingleton(configuration)
                .AddSingleton((IConfiguration)configuration)
                .ConfigureAutomapper()
                .ConfigureAllRepositories()              
                .AddMemoryCache()
                .AddControllers();
            return serviceCollection;
        }
        public static IServiceCollection ConfigureAllRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Employee, Guid>, EFRepository<Employee, Guid>>();
            services.AddScoped<IRepository<Role, Guid>, EFRepository<Role, Guid>>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IRepository<Preference, Guid>, EFRepository<Preference, Guid>>();
            services.AddScoped<IRepository<PromoCode, Guid>, EFRepository<PromoCode, Guid>>();
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IRepository<CustomerPreference, Guid>, EFRepository<CustomerPreference, Guid>>();

            return services;
        }
        public static IServiceCollection ConfigureInMemoryContext(this IServiceCollection services)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDb", builder => { });
                options.UseInternalServiceProvider(serviceProvider);
            });
            services.AddTransient<DbContext, DataContext>();
            return services;
        }
        private static IServiceCollection ConfigureAutomapper(this IServiceCollection services) => services
            .AddAutoMapper(typeof(Program));
  
     
    }
   
}
