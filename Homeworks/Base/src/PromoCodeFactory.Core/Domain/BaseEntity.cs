using System;

namespace PromoCodeFactory.Core.Domain
{
    public abstract  class BaseEntity
    {
        public bool IsDeleted { get; set; }
        public Guid Id { get; set; }
    }
}