using PromoCodeFactory.Core.Domain.Administration;
using System;

namespace PromoCodeFactory.Core.Domain.DTOs
{
    public class UpdateEmployeeDto : Employee
    {
        public UpdateEmployeeDto() { }
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    }
}