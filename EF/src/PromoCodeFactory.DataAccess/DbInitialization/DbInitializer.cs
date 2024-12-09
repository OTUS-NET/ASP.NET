using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.DbInitialization
{
    public class DbInitializer 
    {
        protected readonly DataContext _dbContext;

        public DbInitializer(DataContext dbContext) => _dbContext = dbContext;

        public void Initialize()
        {
            _dbContext.Database.EnsureDeleted();
          
            _dbContext.Database.Migrate();

            _dbContext.AddRange(FakeDataFactory.Employees);
            _dbContext.AddRange(FakeDataFactory.Preferences);
            _dbContext.AddRange(FakeDataFactory.Customers);

            _dbContext.SaveChanges();
        }
    }
}