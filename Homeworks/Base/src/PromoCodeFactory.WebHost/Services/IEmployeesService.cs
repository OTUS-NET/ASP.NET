using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Services;

public interface IEmployeesService
{
    public Task<Employee> GetByIdAsync(Guid id);
    public Task<IEnumerable<Employee>> GetAllAsync();
    public Task<Employee> CreateAsync(CreateOrUpdateEmployeeRequest request);
    public Task<bool> UpdateAsync(Guid id, CreateOrUpdateEmployeeRequest request);
    public Task<bool> DeleteAsync(Guid id);
}