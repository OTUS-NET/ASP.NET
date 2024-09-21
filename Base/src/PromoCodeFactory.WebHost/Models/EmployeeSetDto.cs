using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models;

public record EmployeeSetDto
{
    [StringLength(20, MinimumLength = 2)] 
    public required string FirstName { get; set; }

    [StringLength(20, MinimumLength = 2)]
    public required string LastName { get; set; }

    [StringLength(50, MinimumLength = 5)] 
    public required string Email { get; set; }
    
    public RoleSetDto? Role { get; set; }
}