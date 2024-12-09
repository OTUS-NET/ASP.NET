using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EmployeeRepositoriy : EfRepository<Employee>
    {
        public EmployeeRepositoriy(DataContext dataContext) : base(dataContext) { }
    }
}