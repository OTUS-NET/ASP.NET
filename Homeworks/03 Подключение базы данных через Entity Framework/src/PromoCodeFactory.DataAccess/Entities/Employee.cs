using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Entities;

public class Employee: BaseEntity
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public Guid RoleId { get; set; }

    public Role? Role { get; set; }
}
