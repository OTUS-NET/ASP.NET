using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pcf.Administration.Core.Domain;
using Pcf.Administration.DataAccess.Repositories;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Ef;

namespace Pcf.Administration.DataAccess;

public static class DependencyInjection
{
    public static void AddEfDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<PromoCodeFactoryDbContext>((sp, builder) =>
        {
            var conncetionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("PromocodeFactoryDb")
                ?? throw new InvalidOperationException("Connection string 'PromocodeFactoryDb' is not configured.");
            builder.UseSqlite(conncetionString);
        });

        services.AddScoped<IRepository<Employee>, EmployeeEfRepository>();
        services.AddScoped<IRepository<Role>, EfRepository<PromoCodeFactoryDbContext, Role>>();
    }
}
