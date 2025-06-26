using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.IntegrationTests.Data
{
    public static class TestDataFactory
    {
        public static List<Employee> Employees => new List<Employee>()
        {
            new Employee()
            {
                Id = new ObjectId("000000000000000000000001"),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                RoleId = Roles.FirstOrDefault(x => x.Name == "Admin")!.Id,
                AppliedPromocodesCount = 5
            },
            new Employee()
            {
                Id = new ObjectId("000000000000000000000002"),
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
                Id = new ObjectId("000000000000000000000001"),
                Name = "Admin",
                Description = "Администратор",
            },
            new Role()
            {
                Id = new ObjectId("000000000000000000000002"),
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
        };
    }
}