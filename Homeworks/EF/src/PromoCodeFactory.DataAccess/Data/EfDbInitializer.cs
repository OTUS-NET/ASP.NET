using PromoCodeFactory.DataAccess.Context;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Data
{
    public class EfDbInitializer
        : IDbInitializer
    {
        private readonly DatabaseContext _dataContext;

        public EfDbInitializer(DatabaseContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task InitializeDbAsync()
        {
            // Удаляем базу данных при старте и создаем новую
            await _dataContext.Database.EnsureDeletedAsync();
            await _dataContext.Database.EnsureCreatedAsync();
            //_dataContext.AddRange(FakeDataFactory.Employees);
            //_dataContext.SaveChanges();

            //_dataContext.AddRange(FakeDataFactory.Preferences);
            //_dataContext.SaveChanges();

            //_dataContext.AddRange(FakeDataFactory.Customers);
            //_dataContext.SaveChanges();

            //_dataContext.AddRange(FakeDataFactory.Partners);
            //_dataContext.SaveChanges();
        }
    }
}
