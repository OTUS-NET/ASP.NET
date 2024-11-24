using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static List<Employee> Employees => new List<Employee>()
        {
            new Employee()
            {
                Id = ObjectId.GenerateNewId(DateTime.Now).ToString(),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                Role = Roles.FirstOrDefault(r => r.Name == "Admin"),
                AppliedPromocodesCount = 5
            },
            new Employee()
            {
                Id = ObjectId.GenerateNewId(DateTime.Now).ToString(),
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                Role = Roles.FirstOrDefault(x => x.Name == "PartnerManager"),
                AppliedPromocodesCount = 10
            },
        };

        public static List<Role> Roles => new List<Role>()
        {
            new Role()
            {
                Id = ObjectId.GenerateNewId(DateTime.Now).ToString(),
                Name = "Admin",
                Description = "Администратор",
            },
            new Role()
            {
                Id = ObjectId.GenerateNewId(DateTime.Now).ToString(),
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
        };
    }
}