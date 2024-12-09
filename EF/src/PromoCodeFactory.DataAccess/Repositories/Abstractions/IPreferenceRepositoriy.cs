using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories.Abstractions
{
    public interface IPreferenceRepositoriy : IRepository<Preference>
    {
        Task<Preference?> GetByName(string name);
    }
}