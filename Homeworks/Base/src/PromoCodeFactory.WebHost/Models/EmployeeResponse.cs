using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }

        // FullName разделён на FirstName и LastName.
        // Использование полного имени вместо частей делает
        // невозможным корректное обновление из фронтенда
        // (FullName на бэке надо разделить, но не ясно делить
        // трёх- и более сегментные ФИО: Пхе Мун Сон, Эльчин Тахир оглы и пр.)
        // public string FullName { get; set; }
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string Email { get; set; }

        public List<RoleItemResponse> Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}