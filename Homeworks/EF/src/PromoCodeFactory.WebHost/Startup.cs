using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Repositories;

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
            services.AddControllers();

            // 1. сделать реализацию IRepository в виде EfRepository
            services.AddScoped<IRepository<Role>, EfRepository<Role>>();
            services.AddScoped<IRepository<Employee>, EfRepository<Employee>>();
            services.AddScoped<IRepository<Preference>, EfRepository<Preference>>();
            services.AddScoped<IRepository<Customer>, EfRepository<Customer>>();
            services.AddScoped<IRepository<PromoCode>, EfRepository<PromoCode>>();

            services.AddDbContext<DataContext>(options =>
            {
                // 2. Добавить SQLite в качестве БД
                //options.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));
                options.UseNpgsql(Configuration.GetConnectionString("NpgsqlConnection"));
                options.UseLazyLoadingProxies();
            });

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // 3. База должна удаляться и создаваться каждый раз
                // 8. удаление на каждый запуск уже не должно происходить
                //dbContext.Database.EnsureDeleted();
                //dbContext.Database.EnsureCreated();

                // 8. реализовать две миграции
                // начальную миграцию:
                // dotnet ef migrations add InitialCreate --project "...\ASP.NET\Homeworks\EF\src\PromoCodeFactory.DataAccess\PromoCodeFactory.DataAccess.csproj"
                // миграцию с изменением:
                // dotnet ef migrations add AddCustomerPromoCodesTable --project "...\ASP.NET\Homeworks\EF\src\PromoCodeFactory.DataAccess\PromoCodeFactory.DataAccess.csproj"
                dbContext.Database.Migrate();
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