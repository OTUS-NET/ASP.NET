using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PromoCodeFactory.WebHost.Models
{
    public class CreateEmployeeRequest
    {
        [Description("Имя сотрудника")]
        [DefaultValue("Имя")]
        public string Firstname { get; set; }

        [Description("Фамилия сотрудника")]
        [DefaultValue("Фамилия")] 
        public string Lastname { get; set; }

        [Description("Email")]
        [DefaultValue("Email")]
        public string Email { get; set; }

        [Description("Идентификаторы ролей")]
        public IEnumerable<Guid> RoleIds { get; set; } = new List<Guid>();

        [Description("Количество промокодов")]
        [DefaultValue(0)]
        public int AppliedPromocodesCount { get; set; }
    }
}