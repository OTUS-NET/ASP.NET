using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public Guid RoleId { get; set; }
    }
}
