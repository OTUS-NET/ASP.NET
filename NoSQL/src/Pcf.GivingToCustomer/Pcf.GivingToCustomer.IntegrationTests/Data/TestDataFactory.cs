using System.Collections.Generic;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.IntegrationTests.Data
{
    public static class TestDataFactory
    {
        public static List<Preference> Preferences => new List<Preference>()
        {
            new Preference()
            {
                Id = "4882b11520f095be13af10d9",
                Name = "Театр",
            },
            new Preference()
            {
                Id = "4882b11520f095be13af10d8",
                Name = "Семья",
            },
            new Preference()
            {
                Id = "4882b11520f095be13af10d7",
                Name = "Дети",
            }
        };

        public static List<Customer> Customers
        {
            get
            {
                string customerId = "4882b11520f095be13af10d4";
                var customers = new List<Customer>()
                {
                    new Customer()
                    {
                        Id = customerId,
                        Email = "ivan_sergeev@mail.ru",
                        FirstName = "Иван",
                        LastName = "Петров",
                        Preferences = new List<string>()
                        {
                            Preferences[0].Id,
                            Preferences[1].Id,
                        }
                    }
                };

                return customers;
            }
        }
    }
}