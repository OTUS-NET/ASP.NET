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
            var customerId = ObjectId.GenerateNewId(DateTime.Now+TimeSpan.FromSeconds(1)).ToString();
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
                       Preferences.FirstOrDefault(p => p.Name == "Театр").Id,
                       Preferences.FirstOrDefault(p => p.Name == "Дети").Id      
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