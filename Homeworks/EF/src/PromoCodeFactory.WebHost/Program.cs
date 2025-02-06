using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.WebHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
                //db.Database.Migrate();
                await Seed(scope.ServiceProvider);
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            
            var employeesRepository = scope.ServiceProvider.GetService<EfRepository<Employee, Guid>>();
            await employeesRepository.AddRangeIfNotExistsAsync(FakeDataFactory.Employees);
            await employeesRepository.SaveChangesAsync();
            
            var rolesRepository = scope.ServiceProvider.GetService<EfRepository<Role, Guid>>();
            await rolesRepository.AddRangeIfNotExistsAsync(FakeDataFactory.Roles);
            await rolesRepository.SaveChangesAsync();
            
            var customersRepository = scope.ServiceProvider.GetService<EfRepository<Customer, Guid>>();
            await customersRepository.AddRangeIfNotExistsAsync(FakeDataFactory.Customers);
            await customersRepository.SaveChangesAsync();
            
            var preferencesRepository = scope.ServiceProvider.GetService<EfRepository<Preference, Guid>>();
            await preferencesRepository.AddRangeIfNotExistsAsync(FakeDataFactory.Preferences);
            await preferencesRepository.SaveChangesAsync();
        }
    }
}
