namespace Pcf.Administration.DataAccess.Data
{
    public class EfDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;

        public EfDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
            _dataContext.Database.AutoTransactionBehavior = Microsoft.EntityFrameworkCore.AutoTransactionBehavior.Never;
        }
        
        public void InitializeDb()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            //foreach (var emp in FakeDataFactory.Employees)
            //    _dataContext.Add(emp);
            _dataContext.AddRange(FakeDataFactory.Employees);
            _dataContext.SaveChanges();
        }
    }
}