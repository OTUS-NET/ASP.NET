using MongoDB.Bson;

namespace Pcf.Administration.WebHost.Models
{
    public class EmployeeResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public RoleItemResponse Role { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}