using System;

namespace PromoCodeFactory.Core.Domain
{
    public abstract  class BaseEntity
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedAtUtc { get; set; }
        
        public DateTime UpdatedAtUtc { get; set; }
    }
}