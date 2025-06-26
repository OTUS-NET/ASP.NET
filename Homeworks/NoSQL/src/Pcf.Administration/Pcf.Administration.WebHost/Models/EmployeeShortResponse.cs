using System;
using MongoDB.Bson;

namespace Pcf.Administration.WebHost.Models
{
    public class EmployeeShortResponse
    {
        public ObjectId Id { get; set; }
        
        public string FullName { get; set; }

        public string Email { get; set; }
    }
}