using Pcf.Common.Core.Domain;

namespace Pcf.GivingToCustomer.Core.Domain;

public class Preference : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<Customer> Customers { get; set; } = [];
}
