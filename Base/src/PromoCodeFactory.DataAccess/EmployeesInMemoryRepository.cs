using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess
{
    public class EmployeesInMemoryRepository : InMemoryRepository<Employee>, IEmployeesRepository
    {
        public EmployeesInMemoryRepository(IEnumerable<Employee> data) : base(data)
        {
        }
        public  Task<Employee> UpdateAsync(Guid id,Employee employee) 
        {
            Monitor.Enter(lockObj);

            Employee updateEmployee =  Data.First(x => x.Id == id);
            updateEmployee.FirstName = employee.FirstName;
            updateEmployee.LastName = employee.LastName;
            updateEmployee.Email = employee.Email;
            if(employee.Roles?.Count != 0) updateEmployee.Roles = employee.Roles;
            updateEmployee.AppliedPromocodesCount = employee.AppliedPromocodesCount;

            Monitor.Exit(lockObj);

            return Task.FromResult(updateEmployee);
        }
    }
}
