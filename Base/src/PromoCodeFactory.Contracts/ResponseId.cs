namespace PromoCodeFactory.Contracts;

public record ResponseId<T>
{
    public required T Id { get; set; }
}