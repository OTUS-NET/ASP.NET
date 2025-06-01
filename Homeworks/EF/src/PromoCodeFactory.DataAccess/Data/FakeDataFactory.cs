using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static IEnumerable<Role> Roles => new List<Role>()
        {
            new Role()
            {
                Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                Name = "Admin",
                Description = "Администратор"
            },
            new Role()
            {
                Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
        };

        public static IEnumerable<Employee> Employees => new List<Employee>()
        {
            new Employee()
            {
                Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                RoleId = Roles.First(r => r.Name == "Admin").Id,
                AppliedPromocodesCount = 5
            },
            new Employee()
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                RoleId = Roles.First(r => r.Name == "PartnerManager").Id,
                AppliedPromocodesCount = 10
            },
        };

        public static IEnumerable<Preference> Preferences => new List<Preference>()
        {
            new Preference()
            {
                Id = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                Name = "Театр",
            },
            new Preference()
            {
                Id = Guid.Parse("c4bda62e-fc74-4256-a956-4760b3858cbd"),
                Name = "Семья",
            },
            new Preference()
            {
                Id = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                Name = "Дети",
            }
        };

        public static IEnumerable<Customer> Customers
        {
            get
            {
                var customers = new List<Customer>()
                {
                    new Customer()
                    {
                        Id = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                        Email = "ivan_sergeev@mail.ru",
                        FirstName = "Иван",
                        LastName = "Петров"
                    },
                    new Customer()
                    {
                        Id = Guid.Parse("de687431-e400-41a9-a0bd-c484b0d977dc"),
                        Email = "maria_ivanova@mail.ru",
                        FirstName = "Мария",
                        LastName = "Иванова"
                    }
                };

                return customers;
            }
        }

        public static IEnumerable<CustomerPreference> CustomerPreferences
        {
            get
            {
                var customerPreferences = new List<CustomerPreference>()
                {
                    new CustomerPreference()
                    {
                        CustomerId = Customers.First(c => c.Email == "ivan_sergeev@mail.ru").Id,
                        PreferenceId = Preferences.First(p => p.Name == "Театр").Id
                    },
                    new CustomerPreference()
                    {
                        CustomerId = Customers.First(c => c.Email == "ivan_sergeev@mail.ru").Id,
                        PreferenceId = Preferences.First(p => p.Name == "Дети").Id
                    },
                    new CustomerPreference()
                    {
                        CustomerId = Customers.First(c => c.Email == "maria_ivanova@mail.ru").Id,
                        PreferenceId = Preferences.First(p => p.Name == "Семья").Id
                    },
                    new CustomerPreference()
                    {
                        CustomerId = Customers.First(c => c.Email == "maria_ivanova@mail.ru").Id,
                        PreferenceId = Preferences.First(p => p.Name == "Дети").Id
                    }
                };

                return customerPreferences;
            }
        }

        public static IEnumerable<PromoCode> PromoCodes
        {
            get
            {
                var promoCodes = new List<PromoCode>()
                {
                    new PromoCode()
                    {
                       Id = Guid.Parse("92a6e829-ec25-43a0-9a28-c9988370160a"),
                       Code = "24PREWAZJ",
                       ServiceInfo = "Скидка 30% на оперу",
                       PartnerName = "Мариинский театр",
                       BeginDate = DateTime.UtcNow.AddDays(7),
                       EndDate = DateTime.UtcNow.AddDays(30),
                       PreferenceId = Preferences.First(p => p.Name == "Театр").Id,
                       CustomerId = Customers.First(c => c.Email == "ivan_sergeev@mail.ru").Id
                    },
                    new PromoCode()
                    {
                       Id = Guid.Parse("4605293a-64d2-48a0-bcc7-ad250fae1e20"),
                       Code = "l12ymrew",
                       ServiceInfo = "Скидка 20% на семейный тариф",
                       PartnerName = "Билайн",
                       BeginDate = DateTime.UtcNow,
                       EndDate = DateTime.UtcNow.AddDays(20),
                       PreferenceId = Preferences.First(p => p.Name == "Семья").Id,
                       CustomerId = Customers.First(c => c.Email == "ivan_sergeev@mail.ru").Id
                    },
                    new PromoCode()
                    {
                       Id = Guid.Parse("d840538c-80db-4953-b6ee-51b02e03f710"),
                       Code = "MUE82BN6",
                       ServiceInfo = "Скидка 15% на конструктор",
                       PartnerName = "Детский мир",
                       BeginDate = DateTime.UtcNow.AddDays(6),
                       EndDate = DateTime.UtcNow.AddDays(40),
                       PreferenceId = Preferences.First(p => p.Name == "Дети").Id,
                       CustomerId = Customers.First(c => c.Email == "ivan_sergeev@mail.ru").Id
                    }
                };

                return promoCodes;
            }
        }
    }
}