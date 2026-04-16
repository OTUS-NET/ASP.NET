using System.ComponentModel.DataAnnotations.Schema;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Entities;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
