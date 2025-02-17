using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Repositories.Implementations;
using PromoCodeFactory.Services.Abstractions;
using PromoCodeFactory.Services.Implementations;
using PromoCodeFactory.Services.Repositories.Abstractions;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Settings;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var applicationSettings = _configuration.Get<ApplicationSettings>();
            
            services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlite(applicationSettings.ConnectionString);
            });
            
            InstallRepositories(services);
            InstallServices(services);
            InstallAutomapper(services);

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
        }

        private static void InstallRepositories(IServiceCollection services)
        {
            services.AddTransient<ICustomerRepository, CustomerRepository>()
                .AddTransient<ICustomerUnitOfWork, CustomerUnitOfWork>()
                .AddTransient<IEmployeeRepository, EmployeeRepository>()
                .AddTransient<IPreferenceRepository, PreferenceRepository>()
                .AddTransient<IPromoCodeRepository, PromoCodeRepository>()
                .AddTransient<IPromoCodeUnitOfWork, PromoCodeUnitOfWork>()
                .AddTransient<IRoleRepository, RoleRepository>();
        }
        
        private static void InstallServices(IServiceCollection services)
        {
            services.AddTransient<ICustomerService, CustomerService>()
                .AddTransient<IPreferenceService, PreferenceService>()
                .AddTransient<IPromoCodeService, PromoCodeService>();
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
                
                cfg.AddProfile<CustomerMappingsProfile>();
                cfg.AddProfile<EmployeeMappingProfile>();
                cfg.AddProfile<PreferenceMappingsProfile>();
                cfg.AddProfile<PromoCodeMappingsProfile>();
                cfg.AddProfile<RoleMappingProfile>();
                cfg.AddProfile<Services.Implementations.Mapping.CustomerMappingsProfile>();
                cfg.AddProfile<Services.Implementations.Mapping.PreferenceMappingsProfile>();
                cfg.AddProfile<Services.Implementations.Mapping.PromoCodeMappingsProfile>();
            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}