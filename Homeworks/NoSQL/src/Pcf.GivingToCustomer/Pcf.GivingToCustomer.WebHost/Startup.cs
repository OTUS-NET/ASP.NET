using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.Integration;

namespace Pcf.GivingToCustomer.WebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            MongoDbMappings.Register();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddMvcOptions(x=> 
                x.SuppressAsyncSuffixInActionNames = false);

            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDb"));

            services.AddSingleton<IMongoClient>(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
                return new MongoClient(opts.ConnectionString);
            });
                //x.UseSqlite("Filename=PromocodeFactoryGivingToCustomerDb.sqlite");
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
                return sp.GetRequiredService<IMongoClient>().GetDatabase(opts.DatabaseName);
            });

            services.AddScoped<IRepository<Preference>>(sp =>
                new MongoRepository<Preference>(sp.GetRequiredService<IMongoDatabase>(), "Preferences"));
            services.AddScoped<IRepository<Customer>, CustomerMongoRepository>();
            services.AddScoped<IRepository<PromoCode>, PromoCodeMongoRepository>();

            services.AddScoped<INotificationGateway, NotificationGateway>();
            services.AddScoped<IDbInitializer, MongoDbInitializer>();

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Giving To Customer API Doc";
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
