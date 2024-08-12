using System;

namespace PromoCodeFactory.Service.Employers.ViewModel
{
    public class EmployeeShortResponse
    {
        public Guid Id { get; set; }
        
        public string FullName { get; set; }

        public string Email { get; set; }
    }
}