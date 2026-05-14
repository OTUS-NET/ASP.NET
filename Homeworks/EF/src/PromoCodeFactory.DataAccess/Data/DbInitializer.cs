namespace PromoCodeFactory.DataAccess.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PromoCodeFactoryContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Roles.AddRange(FakeDataFactory.Roles);
            context.Preferences.AddRange(FakeDataFactory.Preferences);
            context.Customers.AddRange(FakeDataFactory.Customers);
            context.Employees.AddRange(FakeDataFactory.Employees);
            context.CustomerPreferences.AddRange(FakeDataFactory.CustomerPreferences);
            context.PromoCodes.AddRange(FakeDataFactory.PromoCodes);

            context.SaveChanges();
        }
    }
}