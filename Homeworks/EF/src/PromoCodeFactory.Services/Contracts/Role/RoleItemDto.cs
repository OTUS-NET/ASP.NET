namespace PromoCodeFactory.Services.Contracts.Role;

public class RoleItemDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public required string Description { get; set; }
}