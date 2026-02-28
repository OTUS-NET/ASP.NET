namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class Preference : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<Customer> Customers { get; set; } = [];
}
