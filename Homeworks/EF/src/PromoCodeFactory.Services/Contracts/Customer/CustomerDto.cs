using PromoCodeFactory.Services.Contracts.Preference;
using PromoCodeFactory.Services.Contracts.PromoCode;

namespace PromoCodeFactory.Services.Contracts.Customer;

public class CustomerDto
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required List<PromoCodeShortDto> PromoCodes { get; set; }
    public required List<PreferenceShortDto> Preferences { get; set; }
}