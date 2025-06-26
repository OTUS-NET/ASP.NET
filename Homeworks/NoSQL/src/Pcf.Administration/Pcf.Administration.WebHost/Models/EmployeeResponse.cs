using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.WebHost.Models
{
    public class EmployeeResponse
    {
        public ObjectId Id { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public RoleItemResponse Role { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}