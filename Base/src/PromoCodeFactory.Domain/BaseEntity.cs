using System;

namespace PromoCodeFactory.Core
{
    public abstract  class BaseEntity
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedAtUtc { get; set; }
        
        public DateTime UpdatedAtUtc { get; set; }
    }
}