using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;


namespace PromoCodeFactory.WebHost.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder InitDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<DataContext>();

        //db.Database.EnsureDeleted();
        //db.Database.EnsureCreated();
        db.Database.Migrate();

        SeedingData(db);
        return app;
    }

    private static void SeedingData(DataContext context)
    {
        if (!context.Roles.Any())
        {
            context.AddRange(FakeDataFactory.Roles);
            context.SaveChanges();
        }

        if (!context.Employees.Any())
        {
            context.AddRange(FakeDataFactory.Employees);
            context.SaveChanges();
        }

        if (!context.Customers.Any())
        {
            context.AddRange(FakeDataFactory.Customers);
            context.SaveChanges();
        }

        if (!context.Preferences.Any())
        {
            context.AddRange(FakeDataFactory.Preferences);
            context.SaveChanges();
        }
    }
}
