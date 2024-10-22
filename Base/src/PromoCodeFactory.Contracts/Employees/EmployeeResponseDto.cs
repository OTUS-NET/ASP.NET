using PromoCodeFactory.Contracts.Roles;

namespace PromoCodeFactory.Contracts.Employees;

public class EmployeeResponseDto
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public List<RoleItemResponse> Roles { get; set; }

    public int AppliedPromocodesCount { get; set; }
}