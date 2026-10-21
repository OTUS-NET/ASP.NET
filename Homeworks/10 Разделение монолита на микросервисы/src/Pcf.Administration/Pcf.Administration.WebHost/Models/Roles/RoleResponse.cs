namespace Pcf.Administration.WebHost.Models.Roles;

public record RoleResponse(
    Guid Id,
    string Name,
    string? Description);
