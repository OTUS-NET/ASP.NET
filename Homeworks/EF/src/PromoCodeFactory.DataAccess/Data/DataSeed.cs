using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class DataSeed
    {
        public static async Task GetData(DataContext context)
        {
            if (!await context.Roles.AnyAsync())
                await context.Roles.AddRangeAsync(FakeDataFactory.Roles);

            if (!await context.Employees.AnyAsync())
                await context.Employees.AddRangeAsync(FakeDataFactory.Employees);

            if (!await context.Preferences.AnyAsync())
                await context.Preferences.AddRangeAsync(FakeDataFactory.Preferences);

            if (!await context.Customers.AnyAsync())
                await context.Customers.AddRangeAsync(FakeDataFactory.Customers);

            if (!await context.CustomerPreferences.AnyAsync())
                await context.CustomerPreferences.AddRangeAsync(FakeDataFactory.CustomerPreferences);

            if (!await context.PromoCodes.AnyAsync())
                await context.PromoCodes.AddRangeAsync(FakeDataFactory.PromoCodes);

            await context.SaveChangesAsync();

        }
    }
}
