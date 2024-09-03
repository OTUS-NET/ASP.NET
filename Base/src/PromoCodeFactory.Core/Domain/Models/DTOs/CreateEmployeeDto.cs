using PromoCodeFactory.Core.Domain.Administration;
using System;

namespace PromoCodeFactory.Core.Domain.DTOs
{
    public class CreateEmployeeDto : Employee
    {
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    }
}