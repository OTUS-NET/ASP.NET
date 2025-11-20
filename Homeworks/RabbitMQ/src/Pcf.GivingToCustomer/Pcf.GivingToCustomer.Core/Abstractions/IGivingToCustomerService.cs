using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.Core.Abstractions;

public interface IGivingToCustomerService
{
    Task GiveToCustomer(PromoCode promoCode);
}
