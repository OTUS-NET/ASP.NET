using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Domain.Base
{
    public interface IEntity<out TId> where TId : struct
    {
        TId Id { get; }
    }
}
