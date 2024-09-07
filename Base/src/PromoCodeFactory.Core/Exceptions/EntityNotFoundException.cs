using System;

namespace PromoCodeFactory.Core.Exceptions;

public sealed class EntityNotFoundException : Exception
{
    public EntityNotFoundException() : base()
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(Guid id, string entityName)
        : base($"Entity {entityName} with id {id} not found")
    {
    }
}
