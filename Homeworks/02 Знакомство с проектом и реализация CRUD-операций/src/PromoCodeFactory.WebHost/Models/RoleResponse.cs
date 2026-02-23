namespace PromoCodeFactory.WebHost.Models;

public record RoleResponse(
    Guid Id,
    string Name,
    string? Description);
