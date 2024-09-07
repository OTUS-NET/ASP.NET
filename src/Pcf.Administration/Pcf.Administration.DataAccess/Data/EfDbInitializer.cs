namespace Pcf.Administration.DataAccess.Data
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

            foreach (var item in FakeDataFactory.Employees)
            {
                _dataContext.Employees.Add(item);
            }
            _dataContext.SaveChanges();
        }
    }
}