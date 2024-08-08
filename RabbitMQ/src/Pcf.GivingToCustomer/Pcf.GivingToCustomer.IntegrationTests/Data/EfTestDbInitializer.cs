using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.DataAccess.Data;

namespace Pcf.GivingToCustomer.IntegrationTests.Data
{
    public class EfTestDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;

        public EfTestDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void InitializeDb()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _dataContext.AddRange(TestDataFactory.Preferences);
            _dataContext.SaveChanges();

            _dataContext.AddRange(TestDataFactory.Customers);
            _dataContext.SaveChanges();
        }

        public void CleanDb()
        {
            _dataContext.Database.EnsureDeleted();
        }
    }
}