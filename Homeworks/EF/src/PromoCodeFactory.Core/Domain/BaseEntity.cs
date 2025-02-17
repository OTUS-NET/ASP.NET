using System;
using PromoCodeFactory.Core.Abstractions;

namespace PromoCodeFactory.Core.Domain
{
    public class BaseEntity : IEntity<Guid>
    {
        public Guid Id { get; set; }
    }
}