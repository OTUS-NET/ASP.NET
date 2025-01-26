using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer
        : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public IEnumerable<Preference> Preferences { get; set; } = new List<Preference>();
        public IEnumerable<PromoCode> Promocodes { get; set; } = new List<PromoCode>();
    }
}