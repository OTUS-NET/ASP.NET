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
using MassTransit;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Administration.Core.Services;
using Pcf.Administration.WebHost.Consumers;

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
            services.AddScoped<IAppliedPromocodesService, AppliedPromocodesService>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<PromoCodeReceivedFromPartnerEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = Configuration["RabbitMq:Host"] ?? "localhost";
                    var virtualHost = Configuration["RabbitMq:VirtualHost"] ?? "/";
                    var username = Configuration["RabbitMq:Username"] ?? "guest";
                    var password = Configuration["RabbitMq:Password"] ?? "guest";

                    ushort port = 5672;
                    if (ushort.TryParse(Configuration["RabbitMq:Port"], out var parsedPort))
                        port = parsedPort;

                    cfg.Host(host, port, virtualHost, h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.ReceiveEndpoint("pcf-administration-promocode-received-from-partner", e =>
                    {
                        e.UseMessageRetry(r => r.Intervals(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));
                        e.ConfigureConsumer<PromoCodeReceivedFromPartnerEventConsumer>(context);
                    });
                });
            });

            services.AddDbContext<DataContext>(x =>
            {
                //x.UseSqlite("Filename=PromocodeFactoryAdministrationDb.sqlite");
                x.UseNpgsql(Configuration.GetConnectionString("PromocodeFactoryAdministrationDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Administration API Doc";
                options.Version = "1.0";
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

            dbInitializer.InitializeDb();
        }
    }
}