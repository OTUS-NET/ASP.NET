using System;
using System.Collections.Generic;
using System.Linq;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static List<Employee> Employees => new List<Employee>()
        {
            new Employee()
            {
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                RoleId = Roles.FirstOrDefault(x => x.Name == "Admin")!.Id,
                AppliedPromocodesCount = 5
            },
            new Employee()
            {
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                RoleId = Roles.FirstOrDefault(x => x.Name == "PartnerManager")!.Id,
                AppliedPromocodesCount = 10
            },
        };

        public static List<Role> Roles => new List<Role>()
        {
            new Role()
            {
                Name = "Admin",
                Description = "Администратор",
            },
            new Role()
            {
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
        };
    }
}