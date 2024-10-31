using Pcf.ReceivingFromPartner.DataAccess;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.DataAccess.Data
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
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _dataContext.AddRange(FakeDataFactory.Preferences);
            _dataContext.SaveChanges();

            _dataContext.AddRange(FakeDataFactory.Partners);
            _dataContext.SaveChanges();
        }
    }
}