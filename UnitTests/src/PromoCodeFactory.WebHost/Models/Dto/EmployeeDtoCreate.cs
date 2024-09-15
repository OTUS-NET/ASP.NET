using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PromoCodeFactory.WebHost.Models.Dto
{
    public class EmployeeDtoCreate:EmployeeDtoBase
    {
        public string  RoleName { get; set; }

        private Role RoleItem { get; set; }

        //ToDo: check if EmailExist must be Unique
        public string Validate(IEnumerable<Role> roles)
        {
            if(string.IsNullOrEmpty(FirstName))
                return "Enter Name";
            if (string.IsNullOrEmpty(LastName))
                return "Enter LastName";
            if (string.IsNullOrEmpty(Email))
                return "Enter unique Email";
            if (string.IsNullOrEmpty(RoleName))
                return "Enter RoleName";

            var Role = roles.FirstOrDefault(x => x.Name == RoleName);

            if (Role == null)
                return "Incorrect Role";
            else
            {
                RoleItem= Role;
            }

            return string.Empty;
        }
        
        public Role GetRole()
        {
            return RoleItem;
        }

        public Employee ToEmployee()
        {
            var res=new Employee();

            res.Email = Email;
            res.FirstName = FirstName;
            res.LastName = LastName;
            res.Id = Guid.NewGuid();
            res.Role = RoleItem;
            res.RoleId = RoleItem.Id;
            res.AppliedPromocodesCount = 0;

            return res;
        }
    }
}
