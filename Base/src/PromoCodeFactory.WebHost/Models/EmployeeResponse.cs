using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public List<RoleItemResponse> Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }

        public EmployeeResponse(Employee source)
        {
            Id = source.Id;
            Email = source.Email;
            Roles = source.Roles.ConvertAll(x => new RoleItemResponse(x));
            FullName = source.FullName;
            AppliedPromocodesCount = source.AppliedPromocodesCount;
        }
    }
}