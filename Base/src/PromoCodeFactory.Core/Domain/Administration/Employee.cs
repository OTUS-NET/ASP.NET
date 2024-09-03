using PromoCodeFactory.Core.Domain.DTOs;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee : BaseEntity
    {
        private Employee(Employee employee)
        {
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            Email = employee.Email;
            AppliedPromocodesCount = employee.AppliedPromocodesCount;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public int AppliedPromocodesCount { get; set; }

        // navigation property
        public List<Role> Roles { get; set; }

        /// <summary>
        /// Returns new Employee.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static Employee CreateEmployee(CreateEmployeeDto employee)
        {
            var newEmployee = new Employee(employee);
            employee.Id = Guid.NewGuid();
            return newEmployee;
        }
    }
}