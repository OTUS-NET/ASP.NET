using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.Identity.UI.Services;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Models.Responses
{
    public class CustomerResponse
    {
        /// <summary>
        /// Previous
        /// </summary>
        //public Guid Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string Email { get; set; }
        //public List<PreferenceResponse> Preferences { get; set; }
        //public List<PromoCodeShortResponse> PromoCodes { get; set; }

        //public CustomerResponse()
        //{

        //}

        //public CustomerResponse(Customer customer)
        //{
        //    Id = customer.Id;
        //    Email = customer.Email;
        //    FirstName = customer.FirstName;
        //    LastName = customer.LastName;
        //    Preferences = customer.Preferences.Select(x => new PreferenceResponse()
        //    {
        //        Id = x.PreferenceId,
        //        Name = x.Preference.Name
        //    }).ToList();
        //}
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<PreferenceResponse> Preferences { get; set; }
        public IEnumerable<PromoCodeShortResponse> PromoCodes { get; set; }
    }
}
