namespace PromoCodeFactory.WebHost.Models;

public record EmployeeResponse(
    Guid Id,
    string FullName,
    string Email,
    RoleResponse Role,
    int AppliedPromocodesCount);
