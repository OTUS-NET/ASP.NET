using System;

namespace PromoCodeFactory.WebHost.Models.Responses
{
    public class CustomerShortResponse
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Previous
        /// </summary>
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}