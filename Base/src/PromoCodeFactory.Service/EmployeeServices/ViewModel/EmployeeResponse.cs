using PromoCodeFactory.Service.RoleServices.ViewModel;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Service.Employers.ViewModel
{
    public abstract class EmployeeBase
    {
        public Guid Id { get; set; }
    }

    public class EmployeeRequest : EmployeeBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public IEnumerable<RoleItemResponse> Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }

    public class EmployeeResponse : EmployeeBase
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public List<RoleItemResponse> Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }

    public class EmployeeCreateRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public List<Guid> Roles { get; set; } = new List<Guid>();

        public int AppliedPromocodesCount { get; set; }
    }
}