namespace Pcf.ReceivingFromPartner.Integration.Administration;

internal sealed record EmployeeResponse(Guid Id, string FullName, string Email, RoleResponse Role);

internal sealed record RoleResponse(Guid Id, string Name, string? Description);
