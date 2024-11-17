//using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models.Requests
{
    public class CreateOrEditEmployeeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NamesRole { get; set; }
        public int AppliedPromocodesCount { get; set; }
    }

}