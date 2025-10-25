using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.GrpcHost.Services;

namespace Pcf.GivingToCustomer.GrpcHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped<IDbInitializer, EfDbInitializer>();
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("PromocodeFactoryGivingToCustomerDb"));
                options.UseSnakeCaseNamingConvention();
                options.UseLazyLoadingProxies();
            });

            builder.Services.AddGrpc();
            builder.Services.AddGrpcReflection();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.MapGrpcService<PreferenceGrpcService>();
            app.MapGrpcService<CustomerGrpcService>();
            app.MapGrpcService<PromoCodeGrpcService>();
            app.MapGrpcReflectionService();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                dbInitializer.InitializeDb();
            }
            app.Run();
        }
    }
}