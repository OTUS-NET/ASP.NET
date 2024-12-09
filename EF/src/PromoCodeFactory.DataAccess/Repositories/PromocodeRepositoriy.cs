using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class PromocodeRepositoriy : EfRepository<PromoCode>
    {
        public PromocodeRepositoriy(DataContext dataContext) : base(dataContext) { }

        public override async Task<PromoCode> CreateAsync(PromoCode obj, CancellationToken cancellationToken = default)
        {
            if (obj.Preference!=null)
                _dbContext.Preferences.Attach(obj.Preference);
            if (obj.PartnerManager!=null)
                _dbContext.Employees.Attach(obj.PartnerManager);

            var entity = (await _entitySet.AddAsync(obj, cancellationToken)).Entity;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}