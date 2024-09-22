using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.Base;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Helpers
{
    public static class MigrationsManager
    {
        public static void MigrateDatabase<TDbContext>(this IHost host) 
            where TDbContext: DbContext
        {
            var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            //context.Database.EnsureDeleted();
            context.Database.Migrate();
            //Seed(scope.ServiceProvider);
        }

        public static void Seed(IServiceProvider serviceProvider) 
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<DataContext>();

                context.AddRange(FakeDataFactory.Roles);
                context.AddRange(FakeDataFactory.Employees);
                context.AddRange(FakeDataFactory.Preferences);
                context.AddRange(FakeDataFactory.Customers);
                context.AddRange(FakeDataFactory.CustomerPreferences);
                context.AddRange(FakeDataFactory.PromoCodes);
                context.SaveChanges();
            };
        }
    }
}
