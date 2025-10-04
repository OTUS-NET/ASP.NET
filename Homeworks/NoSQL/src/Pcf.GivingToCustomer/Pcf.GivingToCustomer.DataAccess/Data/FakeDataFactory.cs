using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static List<Customer> Customers
        {
            get
            {
                var customerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
                var customers = new List<Customer>()
                {
                    new Customer()
                    {
                        Id = customerId,
                        Email = "ivan_sergeev@mail.ru",
                        FirstName = "Иван",
                        LastName = "Петров",
                        Preferences = new List<CustomerPreference>()
                        {
                            new CustomerPreference()
                            {
                                CustomerId = customerId,
                                PreferenceId = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84")
                            },
                            new CustomerPreference()
                            {
                                CustomerId = customerId,
                                PreferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c")
                            }
                        }
                    }
                };

                return customers;
            }
        }
    }
}