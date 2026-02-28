namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class Customer : BaseEntity
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public required string Email { get; set; }

    public ICollection<Preference> Preferences { get; set; } = [];

    public ICollection<PromoCode> PromoCodes { get; set; } = [];
}
