namespace PromoCodeFactory.WebHost.Models;

public abstract record EmployeeRequest
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
}

/// <summary>
/// Запрос на создание нового пользователя
/// </summary>
public record EmployeeCreationRequest : EmployeeRequest;
    
/// <summary>
/// Запрос на изменение пользователя
/// </summary>
public record EmployeeUpdateRequest : EmployeeRequest;