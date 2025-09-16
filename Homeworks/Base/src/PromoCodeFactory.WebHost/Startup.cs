using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Mappers;
using PromoCodeFactory.WebHost.Mappers.EmployeeMappers;
using PromoCodeFactory.WebHost.Mappers.RoleMappers;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(typeof(IRepository<Employee>), _ => 
                new InMemoryRepository<Employee>(FakeDataFactory.Employees));
            services.AddSingleton(typeof(IRepository<Role>), _ => 
                new InMemoryRepository<Role>(FakeDataFactory.Roles));

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });

            services.AddSingleton<IMapper<Role, RoleItemResponse>, RoleToRoleItemResponseMapper>();
            services.AddSingleton<IMapper<RoleItemResponse, Role>, RoleItemToRoleResponseMapper>();
            services.AddSingleton<IRoleMappers, RoleMappers>();
            
            services.AddSingleton<IMapper<Employee, EmployeeResponse>, EmployeeToEmployeeResponseMapper>();
            services.AddSingleton<IMapper<Employee, EmployeeShortResponse>, EmployeeToEmployeeShortResponseMapper>();
            services.AddSingleton<IMapper<EmployeeCreationRequest, Employee>, EmployeeCreationRequestToEmployeeMapper>();
            services.AddSingleton<IMapper<EmployeeUpdateRequest, Employee>, EmployeeUpdateRequestToEmployeeMapper>();
            services.AddSingleton<IEmployeeMappers, EmployeeMappers>();
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