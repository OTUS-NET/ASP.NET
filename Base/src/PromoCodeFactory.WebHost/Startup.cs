using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Services;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Middleware;
using PromoCodeFactory.WebHost.Services;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IService<Employee>, EmployeeService>();
            services.AddSingleton(typeof(IRepository<Employee>), (sp) => 
                new InMemoryRepository<Employee>(FakeDataFactory.Employees, sp.GetRequiredService<ILogger<Employee>>()));
            services.AddSingleton(typeof(IRepository<Role>), (sp) => 
                new InMemoryRepository<Role>(FakeDataFactory.Roles, sp.GetRequiredService<ILogger<Role>>()));

            services.AddProblemDetails(); 
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
        }

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

            app.UseExceptionHandler();

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