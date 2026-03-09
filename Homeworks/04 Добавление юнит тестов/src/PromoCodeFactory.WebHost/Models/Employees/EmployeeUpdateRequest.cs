using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models.Employees;

public record EmployeeUpdateRequest(
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    string FirstName,

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    string LastName,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,

    [Required(ErrorMessage = "RoleId is required")]
    Guid RoleId);
