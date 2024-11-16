using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Services
{
    public class EmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Role> _rolesRepository;

        public EmployeeService(IRepository<Employee> employeeRepository, IRepository<Role> rolesRepository)
        {
            _employeeRepository = employeeRepository;
            _rolesRepository = rolesRepository;
        }

        public async Task<List<EmployeeShortResponse>> GetAllEmployeesAsync(CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetAllAsync(cancellationToken);
            return employees.Select(x => new EmployeeShortResponse
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName
            }).ToList();
        }

        public async Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if (employee == null)
                return null;

            return new EmployeeResponse
            {
                Id = employee.Id,
                Email = employee.Email,
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount,
                Roles = employee.Roles.Select(r => new RoleItemResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToList()
            };
        }

        public async Task<EmployeeResponse> CreateEmployeeAsync(EmployeeCreateRequest request, CancellationToken cancellationToken)
        {
            // Проверка email
            if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                throw new ArgumentException("Некорректный формат email.");
            }

            // Проверка и получение ролей
            var validRoleIds = request.RoleIdList
                .Where(roleId => Guid.TryParse(roleId, out _))
                .Select(Guid.Parse)
                .ToList();

            var roles = await _rolesRepository.GetAllAsync(cancellationToken);
            var employeeRoles = roles.Where(r => validRoleIds.Contains(r.Id)).ToList();

            var missingRoles = validRoleIds.Except(employeeRoles.Select(r => r.Id)).ToList();
            if (missingRoles.Any())
            {
                throw new ArgumentException($"Роли не найдены для Id: {string.Join(", ", missingRoles)}");
            }

            var newEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Roles = employeeRoles,
                AppliedPromocodesCount = 0
            };

            await _employeeRepository.AddAsync(newEmployee, cancellationToken);

            return new EmployeeResponse
            {
                Id = newEmployee.Id,
                Email = newEmployee.Email,
                FullName = newEmployee.FullName,
                AppliedPromocodesCount = newEmployee.AppliedPromocodesCount,
                Roles = newEmployee.Roles.Select(r => new RoleItemResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToList()
            };
        }

        public async Task<bool> DeleteEmployeeAsync(Guid id, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if (employee == null)
                return false;

            await _employeeRepository.DeleteAsync(id, cancellationToken);
            return true;
        }

        public async Task UpdateEmployeeAsync(Guid id, EmployeeUpdateRequest request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if (employee == null)
            {
                throw new ArgumentException("Сотрудник не найден.");
            }

            if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                throw new ArgumentException("Некорректный формат email.");
            }

            var validRoleIds = request.RoleIdList
                .Where(roleId => Guid.TryParse(roleId, out _))
                .Select(Guid.Parse)
                .ToList();

            var roles = await _rolesRepository.GetAllAsync(cancellationToken);
            var employeeRoles = roles.Where(r => validRoleIds.Contains(r.Id)).ToList();

            var missingRoles = validRoleIds.Except(employeeRoles.Select(r => r.Id)).ToList();
            if (missingRoles.Any())
            {
                throw new ArgumentException($"Роли не найдены для Id: {string.Join(", ", missingRoles)}");
            }

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Roles = employeeRoles;

            await _employeeRepository.UpdateAsync(employee, cancellationToken);
        }
    }
}
