using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Entities;

public class Customer: BaseEntity
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public ICollection<Preference> Preferences { get; set; } = [];

    public ICollection<CustomerPromoCode> CustomerPromoCodes { get; set; } = [];
}
