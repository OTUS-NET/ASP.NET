using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class RoleRepositoriy : EfRepository<Role>
    {
        public RoleRepositoriy(DataContext dataContext) : base(dataContext) { }
    }
}