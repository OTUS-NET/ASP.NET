using Pcf.GivingToCustomer.WebHost.Models.Preferences;
using Pcf.GivingToCustomer.WebHost.Models.PromoCodes;

namespace Pcf.GivingToCustomer.WebHost.Models.Customers;

public record CustomerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    IReadOnlyCollection<PreferenceShortResponse> Preferences,
    IReadOnlyCollection<CustomerPromoCodeResponse> PromoCodes);
