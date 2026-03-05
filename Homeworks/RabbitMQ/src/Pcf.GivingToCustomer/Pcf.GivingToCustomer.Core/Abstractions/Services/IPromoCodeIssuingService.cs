using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Abstractions.Services.Models;

namespace Pcf.GivingToCustomer.Core.Abstractions.Services
{
    public interface IPromoCodeIssuingService
    {
        Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeToCustomersWithPreferenceCommand command);
    }
}

