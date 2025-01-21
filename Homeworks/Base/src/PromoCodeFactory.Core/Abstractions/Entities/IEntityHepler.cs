using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Entities
{
    public interface IEntityHepler<T> where T : BaseEntity
    {
        T UpdateEntity(T actual, T excepted);
    }
}
