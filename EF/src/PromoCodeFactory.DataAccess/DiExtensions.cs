using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.DataAccess
{
    public static class DiExtensions
    {
        public static void EnsureDeletedAndMigrateCompleted(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            db.Database.EnsureDeletedAsync();
            db.Database.MigrateAsync();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Employee>, EfCoreRepository<Employee>>();
            services.AddScoped<IRepository<Customer>, CustomersRepository>();
            services.AddScoped<IRepository<Role>, EfCoreRepository<Role>>();
            services.AddScoped<IRepository<Preference>, EfCoreRepository<Preference>>();
            services.AddScoped<IRepository<PromoCode>, PromoCodeRepository>();
        }
    }
}
