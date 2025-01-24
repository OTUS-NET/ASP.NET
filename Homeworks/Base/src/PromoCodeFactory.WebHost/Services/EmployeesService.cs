using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Services;

public class EmployeesService : IEmployeesService
{
    private readonly IRepository<Employee> _employeesRepository;
    private readonly IRolesService _rolesService;

    public EmployeesService(IRepository<Employee> employeesRepository, IRolesService rolesService)
    {
        _employeesRepository = employeesRepository;
        _rolesService = rolesService;
    }

    public async Task<Employee> GetByIdAsync(Guid id) => await _employeesRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Employee>> GetAllAsync() => await _employeesRepository.GetAllAsync();

    public async Task<Employee> CreateAsync(CreateOrUpdateEmployeeRequest request)
    {
        Employee newEmployee = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            AppliedPromocodesCount = 0,
            Roles = new List<Role>(request.RoleIds.Count)
        };

        foreach (var roleId in request.RoleIds.Distinct())
        {
            var role = await _rolesService.GetByIdAsync(roleId);
            if (role == null)
                return null;
            
            newEmployee.Roles.Add(role);
        }

        await _employeesRepository.AddAsync(newEmployee);
        return newEmployee;
    }

    public async Task<bool> UpdateAsync(Guid id, CreateOrUpdateEmployeeRequest request)
    {
        var employee = await GetByIdAsync(id);
        if (employee == null)
        {
            return false;
        }

        var newRoles = new List<Role>(request.RoleIds.Count);
        foreach (var roleId in request.RoleIds.Distinct())
        {
            var role = await _rolesService.GetByIdAsync(roleId);
            if (role == null)
            {
                return false;
            }

            newRoles.Add(role);
        }

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.Roles.Clear();
        employee.Roles.AddRange(newRoles);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await GetByIdAsync(id);
        if (employee == null)
        {
            return false;
        }

        return await _employeesRepository.RemoveAsync(id);
    }
}