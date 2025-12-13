using System;

namespace PromoCodeFactory.Core.Exceptions
{
    public class NotFoundEntityException : Exception
    {
        public NotFoundEntityException(string entity) : base($"Entity \"{entity}\" not found.") { }
    }
}
