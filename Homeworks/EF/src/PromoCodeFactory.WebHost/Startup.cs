using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.DataAccess.Repositories.EF;
using PromoCodeFactory.WebHost.BusinessLogic;
using static System.Formats.Asn1.AsnWriter;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.AddScoped(typeof(IRepository<Employee>), (x) =>
            //    new InMemoryRepository<Employee>(FakeDataFactory.Employees));
            //services.AddScoped(typeof(IRepository<Role>), (x) =>
            //    new InMemoryRepository<Role>(FakeDataFactory.Roles));
            //services.AddScoped(typeof(IRepository<Preference>), (x) =>
            //    new InMemoryRepository<Preference>(FakeDataFactory.Preferences));
            //services.AddScoped(typeof(IRepository<Customer>), (x) =>
            //    new InMemoryRepository<Customer>(FakeDataFactory.Customers));

            //Реристрация DbContext.
            //TODO: используется заглушка в качестве пути к бд
            services.AddDbContext<DatabaseContext>(options => options.UseSqlite("Data Source=..\\PromoCodeFactory.DataAccess\\DataBase\\LocalDatabase.db"));
            services.AddScoped<DbContext>(p => p.GetRequiredService<DatabaseContext>()); // регистрация моего контекста в качестве общего

            services.AddScoped<IRepository<Customer>, CustomerRepository>();
            services.AddScoped<IRepository<Employee>, EfRepositoryBase<Employee>>();
            services.AddScoped<IRepository<Role>, EfRepositoryBase<Role>>();
            services.AddScoped<IRepository<Preference>, EfRepositoryBase<Preference>>();
            services.AddScoped<IRepository<PromoCode>, PromocodeRepository>();

            services.AddScoped<PromoCodeFactoryBl, PromoCodeFactoryBl>();

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
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