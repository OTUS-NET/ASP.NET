using MongoDB.Driver;
using System.Collections;

namespace Pcf.Administration.DataAccess.Data
{
    public class EfMongoDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;

        public EfMongoDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public void InitializeDb()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();
            
            _dataContext.AddRange(FakeDataFactory.Employees);
            _dataContext.SaveChanges();
        }

        private void EnsureDelete()
        {
            
        }
    }
}