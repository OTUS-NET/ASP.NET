using System;
using System.Threading.Tasks;
using Infrastructure.RabbitMq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.DataAccess;
using Pcf.ReceivingFromPartner.DataAccess.Repositories;
using Pcf.ReceivingFromPartner.DataAccess.Data;
using Pcf.ReceivingFromPartner.Integration;
using Pcf.ReceivingFromPartner.WebHost.Settings;
using RabbitMQ.Client;

namespace Pcf.ReceivingFromPartner.WebHost
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
            services.AddScoped<INotificationGateway, NotificationGateway>();
            services.AddScoped<IDbInitializer, EfDbInitializer>();

            CreateGateways(services).Wait();

            services.AddDbContext<DataContext>(x =>
            {
                //x.UseSqlite("Filename=PromocodeFactoryReceivingFromPartnerDb.sqlite");
                x.UseNpgsql(Configuration.GetConnectionString("PromocodeFactoryReceivingFromPartnerDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Receiving From Partner API Doc";
                options.Version = "1.0";
            });
        }

        private async Task CreateGateways(IServiceCollection services)
        {
            var rmqSettings = Configuration.Get<ApplicationSettings>().RmqSettings;
            var connection = await RabbitMqConfiguration.GetRabbitConnection(rmqSettings);
            var channel = await connection.CreateChannelAsync();
            
            services.AddSingleton<IAdministrationGateway>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                return new AdministrationGateway(
                    ExchangeType.Direct,
                    configuration["AdministrationExchangeName"],
                    configuration["AdministrationRoutingKey"],
                    channel
                );
            });
            
            services.AddSingleton<IGivingPromoCodeToCustomerGateway>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new GivingPromoCodeToCustomerGateway(
                    ExchangeType.Direct,
                    configuration["GivingPromoCodeToCustomerExchangeName"],
                    configuration["GivingPromoCodeToCustomerRoutingKey"],
                    channel
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
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

            app.ApplicationServices.GetRequiredService<IAdministrationGateway>();
            app.ApplicationServices.GetRequiredService<IGivingPromoCodeToCustomerGateway>();

            dbInitializer.InitializeDb();
        }
    }
}