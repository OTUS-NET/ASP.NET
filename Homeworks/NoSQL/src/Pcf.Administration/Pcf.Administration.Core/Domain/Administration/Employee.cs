using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Pcf.Administration.Core.Domain.Administration
{
    public class Employee
        : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public ObjectId RoleId { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}