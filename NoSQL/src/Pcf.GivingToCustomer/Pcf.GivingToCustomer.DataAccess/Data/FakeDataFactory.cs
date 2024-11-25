using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Data;

public static class FakeDataFactory
{        
    public static List<Customer> Customers
    {
        get
        {
            var customerId = ObjectId.GenerateNewId(DateTime.Now).ToString();
            var customers = new List<Customer>()
            {
                new Customer()
                {
                    Id = customerId,
                    Email = "ivan_sergeev@mail.ru",
                    FirstName = "Иван",
                    LastName = "Петров",
                    Preferences = new List<Preference>()
                    {
                        Preferences.FirstOrDefault(p => p.Name == "Театр"),
                        Preferences.FirstOrDefault(p => p.Name == "Дети")
                    }
                }
            };

            return customers;
        }
    }
    public static List<Preference> Preferences => new List<Preference>()
    {
        new Preference()
        {
            Id = ObjectId.GenerateNewId(DateTime.Now).ToString(),
            Name = "Театр",
        },
        new Preference()
        {
            Id = ObjectId.GenerateNewId(DateTime.Now).ToString(),
            Name = "Семья",
        },
        new Preference()
        {
            Id = ObjectId.GenerateNewId(DateTime.Now).ToString(),
            Name = "Дети",
        }
    };
}