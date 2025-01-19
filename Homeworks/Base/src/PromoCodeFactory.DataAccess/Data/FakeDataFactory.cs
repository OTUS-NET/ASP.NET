using System;
using System.Collections.Generic;
using System.Linq;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static IDictionary<Guid, Employee> Employees => new Dictionary<Guid, Employee>()
        {
            {
                Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                new Employee()
                {
                    Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                    Email = "owner@somemail.ru",
                    FirstName = "Иван",
                    LastName = "Сергеев",
                    Roles = new List<Guid>()
                    {
                        Roles.Values.FirstOrDefault(x => x.Name == "Admin").Id
                    },
                    AppliedPromocodesCount = 5
                }
            },
            {
                Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                new Employee()
                {
                    Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                    Email = "andreev@somemail.ru",
                    FirstName = "Петр",
                    LastName = "Андреев",
                    Roles = new List<Guid>()
                    {
                        Roles.Values.FirstOrDefault(x => x.Name == "PartnerManager").Id
                    },
                    AppliedPromocodesCount = 10
                }
            }
            
        };

        public static IDictionary<Guid, Role> Roles => new Dictionary<Guid, Role>()
        {
            {
                Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                new Role()
                {
                    Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                    Name = "Admin",
                    Description = "Администратор",
                }   
            },
            {
                Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                new Role()
                {
                    Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                    Name = "PartnerManager",
                    Description = "Партнерский менеджер"
                }
            }
            
        };
    }
}