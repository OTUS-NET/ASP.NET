namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Role : BaseEntity
    {
        public required string Name { get; set; }

        public required string Description { get; set; }
    }
}