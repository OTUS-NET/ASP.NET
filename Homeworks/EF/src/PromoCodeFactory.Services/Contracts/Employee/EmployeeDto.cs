using PromoCodeFactory.Services.Contracts.Role;

namespace PromoCodeFactory.Services.Contracts.Employee;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }

    public required string Email { get; set; }

    public required RoleItemDto Role { get; set; }

    public int AppliedPromocodesCount { get; set; }
}