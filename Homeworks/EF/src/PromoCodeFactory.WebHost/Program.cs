using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.DataAccess.Data;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            // Инициализация базы данных
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DataContext>();

                // удаляем, если есть и создаем БД заново
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                // Заполняем начальными данными
                await DataSeed.GetData(context);
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}