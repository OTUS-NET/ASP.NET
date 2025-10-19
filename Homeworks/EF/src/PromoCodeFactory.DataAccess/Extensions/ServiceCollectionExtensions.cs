using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PromoCodeFactory.DataAccess.Extensions;

public static class EntityFrameworkInstaller
{
    public static IServiceCollection AddSqlLiteContext(this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<DataContext>(optionsBuilder => optionsBuilder
            .UseSqlite(connectionString)
        );

        return services;
    }
}
