using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class UpdateEmployeeRequest : CreateEmployeeRequest
    {
        public Guid Id { get; set; }
    }
}