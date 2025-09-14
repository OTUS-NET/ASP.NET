namespace PromoCodeFactory.WebHost.Models;

public abstract class EmployeeRequest
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
}

/// <summary>
/// Запрос на создание нового пользователя
/// </summary>
public class EmployeeCreationRequest : EmployeeRequest { }
    
/// <summary>
/// Запрос на изменение пользователя
/// </summary>
public class EmployeeUpdateRequest : EmployeeRequest { }