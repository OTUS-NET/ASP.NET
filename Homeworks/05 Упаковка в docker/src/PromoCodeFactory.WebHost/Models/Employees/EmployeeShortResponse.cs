namespace PromoCodeFactory.WebHost.Models.Employees;

public record EmployeeShortResponse(
    Guid Id,
    string FullName,
    string Email);
