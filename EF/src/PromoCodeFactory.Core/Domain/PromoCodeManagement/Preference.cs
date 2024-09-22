using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference: BaseEntity
    {
        public string Name { get; set; }
        public virtual List<PromoCode> PromoCodes{ get; set; }
        public virtual List<CustomerPreference> Customers { get; set; }
    }
}
