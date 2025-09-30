using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //TODO: Добавить список предпочтений
        public List<PromoCodeShortResponse> PromoCodes { get; set; } = [];
        public List<Guid> Preferences { get; set; } = [];
    }
}