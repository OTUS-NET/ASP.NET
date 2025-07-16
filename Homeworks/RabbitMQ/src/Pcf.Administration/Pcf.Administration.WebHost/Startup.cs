using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Pcf.Administration.DataAccess;
using Pcf.Administration.DataAccess.Repositories;
using Pcf.Administration.DataAccess.Data;
using Pcf.Administration.Core.Abstractions.Repositories;
using System;
using System.Threading.Tasks;
using Infrastructure.RabbitMq;
using Pcf.Administration.WebHost.Services.Employees;
using Pcf.Administration.WebHost.Settings;
using RabbitMQ.Client;

namespace Pcf.Administration.WebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddMvcOptions(x =>
                x.SuppressAsyncSuffixInActionNames = false);
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbInitializer, EfDbInitializer>();
            services.AddDbContext<DataContext>(x =>
            {
                //x.UseSqlite("Filename=PromocodeFactoryAdministrationDb.sqlite");
                x.UseNpgsql(Configuration.GetConnectionString("PromocodeFactoryAdministrationDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            ConfigureRabbitMq(services).Wait();
            services.AddScoped<IEmployeesService, EmployeesService>();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Administration API Doc";
                options.Version = "1.0";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        private async Task ConfigureRabbitMq(IServiceCollection services)
        {
            var rmqSettings = Configuration.Get<ApplicationSettings>().RmqSettings;
            var connection = await RabbitMqConfiguration.GetRabbitConnection(rmqSettings);
            
            var channel = await connection.CreateChannelAsync();
            
            services.AddSingleton<IChannel>(_ => channel);
        }

        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IDbInitializer dbInitializer,
            IEmployeesService employeesService,
            IChannel channel)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            employeesService.Register(
                channel,
                Configuration["AdministrationExchangeName"]!,
                Configuration["AdministrationQueue"]!,
                Configuration["AdministrationRoutingKey"]!);
            
            dbInitializer.InitializeDb();
        }
    }
}