using System.Linq;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class DataContextSeeder
    {
        public static void Seed(DataContext dbContext)
        {
            // Since database is re-created on each app start (homework requirement),
            // this check mainly protects from accidental double seeding.
            if (dbContext.Roles.Any() ||
                dbContext.Employees.Any() ||
                dbContext.Preferences.Any() ||
                dbContext.Customers.Any() ||
                dbContext.CustomerPreferences.Any() ||
                dbContext.Promocodes.Any())
            {
                return;
            }

            dbContext.Roles.AddRange(FakeDataFactory.Roles);
            dbContext.Employees.AddRange(FakeDataFactory.Employees);
            dbContext.Preferences.AddRange(FakeDataFactory.Preferences);
            dbContext.Customers.AddRange(FakeDataFactory.Customers);
            dbContext.CustomerPreferences.AddRange(FakeDataFactory.CustomerPreferences);
            dbContext.Promocodes.AddRange(FakeDataFactory.PromoCodes);

            dbContext.SaveChanges();
        }
    }
}
