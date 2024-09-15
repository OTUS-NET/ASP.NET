using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IEmployeesRepository : IRepository<Employee>
    { 
        Task<Employee> UpdateAsync(Guid id,Employee entity);
    }
}
