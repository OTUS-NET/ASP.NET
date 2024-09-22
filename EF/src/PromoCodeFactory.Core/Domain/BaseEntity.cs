using PromoCodeFactory.Core.Domain.Base;
using System;

namespace PromoCodeFactory.Core.Domain
{
    public abstract  class BaseEntity: IEntity<Guid>
    {
        public Guid Id { get; set; }
    }
}