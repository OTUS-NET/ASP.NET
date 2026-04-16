using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Entities;

public class Preference: BaseEntity
{
    public required string Name { get; set; }
}