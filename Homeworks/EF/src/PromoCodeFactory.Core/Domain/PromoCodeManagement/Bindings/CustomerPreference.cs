using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement.Bindings
{
    /// <summary>
    /// Привязка клиента к предпочтению
    /// </summary>
    public class CustomerPreference
    {
        public Preference Preference { get; set; }
        public Guid PreferenceId { get; set; } //FK

        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; } //FK
    }
}
