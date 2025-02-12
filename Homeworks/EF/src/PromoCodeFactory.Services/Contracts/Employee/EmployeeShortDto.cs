namespace PromoCodeFactory.Services.Contracts.Employee;

public class EmployeeShortDto
{
    public Guid Id { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }
}