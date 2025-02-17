namespace PromoCodeFactory.Core.Abstractions;

public interface IEntity<TPrimaryKey>
{
    public TPrimaryKey Id { get; }
}