using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

public class PreferenceRepository : EfRepository<Preference>, IPreferenceRepository
{
    public PreferenceRepository(DataContext dataContext) : base(dataContext) { }

    public async Task<Preference?> GetByName(string name)
    {
        return await _entitySet.FirstOrDefaultAsync(x => x.Name == name);
    }
}