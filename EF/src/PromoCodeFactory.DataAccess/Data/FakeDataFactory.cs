using System;
using System.Collections.Generic;
using System.Linq;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static IEnumerable<Employee> Employees => new List<Employee>()
        {
            new Employee()
            {
                Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                RoleId =  Roles.FirstOrDefault(x => x.Name == "Admin").Id,
                AppliedPromocodesCount = 5
            },
            new Employee()
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                RoleId = Roles.FirstOrDefault(x => x.Name == "PartnerManager").Id,
                AppliedPromocodesCount = 10
            },
        };

        public static IEnumerable<Role> Roles => new List<Role>()
        {
            new Role()
            {
                Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                Name = "Admin",
                Description = "Администратор",
            },
            new Role()
            {
                Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
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

        public static IEnumerable<Customer> Customers => new List<Customer>(){
                    new Customer()
                    {
                        Id = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                        Email = "ivan_pertov@mail.ru",
                        FirstName = "Иван",
                        LastName = "Петров",

                    } ,
                    new Customer()
                    {
                        Id =  Guid.Parse("5B2C9EA9-6097-4390-B5F4-627EF26479A7"),
                        Email = "petr_sidorov@mail.ru",
                        FirstName = "Петр",
                        LastName = "Сидоров"
                    }
                };

        public static IEnumerable<CustomerPreference> CustomerPreferences => new List<CustomerPreference>()
                {
                    new CustomerPreference()
                    {
                        Id = Guid.Parse("FDDE5C31-F5D3-46C8-96F6-A3AD4CF00E2A"),
                        CustomerId =  Customers.FirstOrDefault(x => x.Email == "ivan_pertov@mail.ru").Id,
                        PreferenceId = Preferences.FirstOrDefault(x => x.Name == "Театр").Id
                    } ,
                    new CustomerPreference()
                    {
                        Id = Guid.Parse("4A6EBD15-FC76-4224-96C1-36F7BDF92B09"),
                        CustomerId =  Customers.FirstOrDefault(x => x.Email == "ivan_pertov@mail.ru").Id,
                        PreferenceId = Preferences.FirstOrDefault(x => x.Name == "Дети").Id
                    }  ,
                    new CustomerPreference()
                    {
                        Id = Guid.Parse("703288C8-2266-45A3-A741-F6AAB74FDDB8"),
                        CustomerId =  Customers.FirstOrDefault(x => x.Email == "petr_sidorov@mail.ru").Id,
                        PreferenceId = Preferences.FirstOrDefault(x => x.Name == "Семья").Id
                    },
                    new CustomerPreference()
                    {
                        Id = Guid.Parse("86FE37BF-E02D-46D4-B3CB-3BF5B15CD717"),
                        CustomerId =  Customers.FirstOrDefault(x => x.Email == "petr_sidorov@mail.ru").Id,
                        PreferenceId = Preferences.FirstOrDefault(x => x.Name == "Дети").Id
                    }
                };
        public static IEnumerable<PromoCode> PromoCodes => new List<PromoCode>()
                {
                    new PromoCode()
                    {
                        Id = Guid.Parse("E3C8B644-20DF-40F9-A3A6-2CAEFF162CC8"),
                        Code = "Carousel_010",
                        ServiceInfo = "Take a ride",
                        PartnerName = "The attractions",
                        BeginDate = DateTime.Now.AddDays(5),
                        EndDate = DateTime.Now.AddDays(35),
                        PreferenceId = Preferences.FirstOrDefault(x => x.Name == "Дети").Id,
                        EmployeeId = Employees.FirstOrDefault(x => x.Email == "andreev@somemail.ru").Id,
                        CustomerId = Customers.FirstOrDefault(x => x.Email == "ivan_pertov@mail.ru").Id,
                    } ,
                    new PromoCode()
                    {   
                        Id = Guid.Parse("13208CF8-E793-4F86-A735-C1716E30EFEA"),
                        Code = "Performance_020",
                        ServiceInfo = "Visiting one person",
                        PartnerName = "The theatre",
                        BeginDate = DateTime.Now.AddDays(5),
                        EndDate = DateTime.Now.AddDays(35),
                        PreferenceId = Preferences.FirstOrDefault(x => x.Name == "Театр").Id,
                        EmployeeId = Employees.FirstOrDefault(x => x.Email == "andreev@somemail.ru").Id,
                        CustomerId = Customers.FirstOrDefault(x => x.Email == "ivan_pertov@mail.ru").Id,
                    }  ,
                    new PromoCode()
                    {
                        Id = Guid.Parse("F28E72FA-5BD0-4F14-8A78-9700ECDAF436"),
                        Code = "Shop_030",
                        ServiceInfo = "Purchase on 1000",
                        PartnerName = "The supermarket",
                        BeginDate = DateTime.Now.AddDays(5),
                        EndDate = DateTime.Now.AddDays(35),
                        PreferenceId = Preferences.FirstOrDefault(x => x.Name == "Семья").Id,
                        EmployeeId = Employees.FirstOrDefault(x => x.Email == "andreev@somemail.ru").Id,
                        CustomerId = Customers.FirstOrDefault(x => x.Email == "petr_sidorov@mail.ru").Id,
                    }
                };
    }
}