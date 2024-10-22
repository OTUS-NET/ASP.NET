using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<CustomerPreference> CustomerPreferences { get; set; }
    }
}