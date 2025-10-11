using Pcf.GivingToCustomer.Core.Models;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Services
{
    public interface IPromoCodeService
    {
        Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request);
    }
}
