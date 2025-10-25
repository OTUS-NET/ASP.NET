using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.GraphQLHost.Data;

namespace Pcf.GivingToCustomer.GraphQLHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("PromocodeFactoryGivingToCustomerDb"));
                options.UseSnakeCaseNamingConvention();
                options.UseLazyLoadingProxies();
            });

            // Регистрация репозиториев
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            // Регистрация инициализатора БД
            builder.Services.AddScoped<IDbInitializer, EfDbInitializer>();

            // Настройка GraphQL сервера
            builder.Services
                .AddGraphQLServer()
                .AddQueryType<Queries>()
                .AddMutationType<Mutations>()
                .AddFiltering()
                .AddSorting()
                .AddProjections();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var initializer = services.GetRequiredService<IDbInitializer>();
                initializer.InitializeDb();
            }

            app.UseRouting();
            app.MapGraphQL();
            app.Run();
        }
    }
}
