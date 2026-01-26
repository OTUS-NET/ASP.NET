namespace PromoCodeFactory.WebHost.Models;

public record EmployeeResponse(
    Guid Id,
    string FullName,
    string Email,
    IReadOnlyCollection<RoleItemResponse> Roles,
    int AppliedPromocodesCount);
