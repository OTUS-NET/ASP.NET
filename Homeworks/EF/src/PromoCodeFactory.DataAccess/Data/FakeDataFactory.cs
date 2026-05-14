using System;
using System.Collections.Generic;
using System.Linq;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static readonly Guid AdminRoleId = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02");
        public static readonly Guid PartnerManagerRoleId = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665");

        public static readonly Guid AdminEmployeeId = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f");
        public static readonly Guid PartnerManagerEmployeeId = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895");

        public static readonly Guid TheatrePreferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c");
        public static readonly Guid FamilyPreferenceId = Guid.Parse("c4bda62e-fc74-4256-a956-4760b3858cbd");
        public static readonly Guid ChildrenPreferenceId = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84");

        public static readonly Guid FirstCustomerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
        public static readonly Guid SecondCustomerId = Guid.Parse("b2b72d40-9c7b-4c66-b8a3-1f6d0e3a1111");

        public static readonly Guid FirstPromoCodeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static IEnumerable<Role> Roles => new List<Role>
        {
            new Role
            {
                Id = AdminRoleId,
                Name = "Admin",
                Description = "Администратор"
            },
            new Role
            {
                Id = PartnerManagerRoleId,
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
        };

        public static IEnumerable<Employee> Employees => new List<Employee>
        {
            new Employee
            {
                Id = AdminEmployeeId,
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                RoleId = AdminRoleId,
                AppliedPromocodesCount = 5
            },
            new Employee
            {
                Id = PartnerManagerEmployeeId,
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                RoleId = PartnerManagerRoleId,
                AppliedPromocodesCount = 10
            }
        };

        public static IEnumerable<Preference> Preferences => new List<Preference>
        {
            new Preference
            {
                Id = TheatrePreferenceId,
                Name = "Театр"
            },
            new Preference
            {
                Id = FamilyPreferenceId,
                Name = "Семья"
            },
            new Preference
            {
                Id = ChildrenPreferenceId,
                Name = "Дети"
            }
        };

        public static IEnumerable<Customer> Customers => new List<Customer>
        {
            new Customer
            {
                Id = FirstCustomerId,
                Email = "ivan_sergeev@mail.ru",
                FirstName = "Иван",
                LastName = "Петров"
            },
            new Customer
            {
                Id = SecondCustomerId,
                Email = "maria@mail.ru",
                FirstName = "Мария",
                LastName = "Иванова"
            }
        };

        public static IEnumerable<CustomerPreference> CustomerPreferences => new List<CustomerPreference>
        {
            new CustomerPreference
            {
                CustomerId = FirstCustomerId,
                PreferenceId = TheatrePreferenceId
            },
            new CustomerPreference
            {
                CustomerId = FirstCustomerId,
                PreferenceId = FamilyPreferenceId
            },
            new CustomerPreference
            {
                CustomerId = SecondCustomerId,
                PreferenceId = ChildrenPreferenceId
            }
        };

        public static IEnumerable<PromoCode> PromoCodes => new List<PromoCode>
        {
            new PromoCode
            {
                Id = FirstPromoCodeId,
                Code = "THEATRE-10",
                ServiceInfo = "Скидка 10% на билеты",
                BeginDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddMonths(1),
                PartnerName = "Big Theatre Partner",
                PartnerManagerId = PartnerManagerEmployeeId,
                PreferenceId = TheatrePreferenceId,
                CustomerId = FirstCustomerId
            }
        };
    }
}