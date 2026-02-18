using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PromoCodeFactory.DataAccess.Data
{
    public class EfDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;

        public EfDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public void InitializeDb()
        {
            _dataContext.Database.Migrate();

            if (_dataContext.Employees.Any())
            {
                return;
            }
            
            _dataContext.AddRange(FakeDataFactory.Employees);
            _dataContext.SaveChanges();
            
            _dataContext.AddRange(FakeDataFactory.Preferences);
            _dataContext.SaveChanges();
            
            _dataContext.AddRange(FakeDataFactory.Customers);
            _dataContext.SaveChanges();
            
            _dataContext.AddRange(FakeDataFactory.Partners);
            _dataContext.SaveChanges();
        }
    }
}