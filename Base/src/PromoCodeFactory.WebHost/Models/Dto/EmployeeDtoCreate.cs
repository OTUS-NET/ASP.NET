using PromoCodeFactory.Core.Domain.Administration;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PromoCodeFactory.WebHost.Models.Dto
{
    public class EmployeeDtoCreate:EmployeeDtoBase
    {
        public List<string> RoleNames { get; set; }

        private List<Role> RoleItems { get; set; }

        //ToDo: check if EmailExist must be Unique and Roles of user too
        public string Validate(IEnumerable<Role> roles)
        {
            if (string.IsNullOrEmpty(FirstName))
                return "Enter Name";
            if (string.IsNullOrEmpty(LastName))
                return "Enter LastName";
            if (string.IsNullOrEmpty(Email))
                return "Enter unique Email";
            if (RoleNames==null||RoleNames.Count==0)
                return "Enter any RoleName";

            RoleItems = new List<Role>();
            foreach (var roleName in RoleNames)
            {
                var Role = roles.FirstOrDefault(x => x.Name == roleName);

                if (Role == null)
                    return $"Incorrect Role: {roleName}";

                RoleItems.Add(Role);
            }    
            

            return string.Empty;
        }

        public List<Role> GetRoles()
        {
            return RoleItems;
        }

        public Employee ToEmployee()
        {
            var res = new Employee();

            res.Email = Email;
            res.FirstName = FirstName;
            res.LastName = LastName;
            res.Id = Guid.NewGuid();
            res.Roles = RoleItems;
            res.AppliedPromocodesCount = 0;

            return res;
        }
    }
}
