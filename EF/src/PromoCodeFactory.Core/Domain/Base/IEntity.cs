namespace PromoCodeFactory.Core.Domain.Base
{
    public interface IEntity<out TId> where TId : struct
    {
        TId Id { get; }
    }
}
