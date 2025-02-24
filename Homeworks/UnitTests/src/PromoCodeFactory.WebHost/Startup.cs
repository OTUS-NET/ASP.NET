using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.Services.Date;
using PromoCodeFactory.Services.Date.Abstractions;
using PromoCodeFactory.Services.Partners;
using PromoCodeFactory.Services.Partners.Abstractions;
using PromoCodeFactory.WebHost.Mapping;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace PromoCodeFactory.WebHost
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
            InstallAutomapper(services);
            services.AddControllers().AddMvcOptions(x=> 
                x.SuppressAsyncSuffixInActionNames = false);
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbInitializer, EfDbInitializer>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IPartnersService, PartnersService>();
            services.AddDbContext<DataContext>(x =>
            {
                x.UseSqlite("Filename=PromoCodeFactoryDb.sqlite");
                //x.UseNpgsql(Configuration.GetConnectionString("PromoCodeFactoryDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
        }
        
        private static void InstallAutomapper(IServiceCollection services)
        {
            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));
        }
        
        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AddGlobalIgnore("Item");
                
                cfg.AddProfile<PromoCodeFactory.Services.Partners.Mapping.PartnerMappingsProfile>();
                cfg.AddProfile<PartnerMappingsProfile>();
            });
            configuration.AssertConfigurationIsValid();
            return configuration;
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