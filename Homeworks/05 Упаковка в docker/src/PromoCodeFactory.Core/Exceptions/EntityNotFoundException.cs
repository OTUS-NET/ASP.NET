namespace PromoCodeFactory.Core.Exceptions;

public class EntityNotFoundException(Type entityType, Guid entityId)
    : Exception($"{entityType.Name} with Id '{entityId}' was not found.")
{
    public Type EntityType { get; } = entityType;
    public Guid EntityId { get; } = entityId;
}

public class EntityNotFoundException<T>(Guid id) : EntityNotFoundException(typeof(T), id)
{
}
