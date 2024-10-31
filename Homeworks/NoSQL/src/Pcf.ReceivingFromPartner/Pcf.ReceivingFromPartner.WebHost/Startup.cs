using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.DataAccess;
using Pcf.ReceivingFromPartner.DataAccess.Data;
using Pcf.ReceivingFromPartner.DataAccess.Repositories;
using Pcf.ReceivingFromPartner.Integration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

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
            services.AddControllers().AddMvcOptions(x=> 
                x.SuppressAsyncSuffixInActionNames = false);
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<INotificationGateway, NotificationGateway>();
            services.AddScoped<IDbInitializer, EfDbInitializer>();

            services.AddHttpClient<IGivingPromoCodeToCustomerGateway,GivingPromoCodeToCustomerGateway>(c =>
            {
                c.BaseAddress = new Uri(Configuration["IntegrationSettings:GivingToCustomerApiUrl"]);
            });
            
            services.AddHttpClient<IAdministrationGateway,AdministrationGateway>(c =>
            {
                c.BaseAddress = new Uri(Configuration["IntegrationSettings:AdministrationApiUrl"]);
            });
            
            services.AddDbContext<DataContext>(x =>
            {
                //x.UseSqlite("Filename=PromocodeFactoryReceivingFromPartnerDb.sqlite");
                x.UseNpgsql(Configuration.GetConnectionString("PromocodeFactoryReceivingFromPartnerDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Receiving From Partner API Doc";
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