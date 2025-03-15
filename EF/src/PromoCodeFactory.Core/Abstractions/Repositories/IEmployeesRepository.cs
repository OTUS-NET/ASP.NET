using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IEmployeesRepository : IRepository<Employee>
    {
        Task<Employee> UpdateAsync(Guid id, Employee entity);
    }
}
