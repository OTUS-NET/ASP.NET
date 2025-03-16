using System;

namespace PromoCodeFactory.WebHost.Models.Response
{
    public class EmployeeShortResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}