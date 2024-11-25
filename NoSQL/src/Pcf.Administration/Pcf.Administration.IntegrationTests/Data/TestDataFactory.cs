using System.Collections.Generic;
using System.Linq;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.IntegrationTests.Data
{
    public static class TestDataFactory
    {
        public static List<Employee> Employees => new List<Employee>()
        {
            new Employee()
            {
                Id = "4882b11520f095be13af10d9",
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                RoleId = Roles.FirstOrDefault(x => x.Name == "Admin").Id,
                AppliedPromocodesCount = 5
            },
            new Employee()
            {
                Id = "98df861cde7d85d280dd43ff",
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                RoleId = Roles.FirstOrDefault(x => x.Name == "PartnerManager").Id,
                AppliedPromocodesCount = 10
            },
        };

        public static List<Role> Roles => new List<Role>()
        {
            new Role()
            {
                Id = "b2c15395fc1bca3910777154",
                Name = "Admin",
                Description = "Администратор",
            },
            new Role()
            {
                Id = "e3dcbce04d1f213e1322a23b",
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
        };
    }
}