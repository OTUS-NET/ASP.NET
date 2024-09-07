using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Services;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;

namespace PromoCodeFactory.WebHost.Services
{
    public class EmployeeService : IService<Employee>
    {
        private readonly IRepository<Employee> _employeesRepository;
        private readonly IRepository<Role> _rolesRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IRepository<Employee> employeesRepository, IRepository<Role> rolesRepository, ILogger<EmployeeService> logger)
        {
            _employeesRepository = employeesRepository;
            _rolesRepository = rolesRepository;
            _logger = logger;
        }


        public async Task<IList<Employee>> GetAllAsync(CancellationToken cts)
        {
            var data = await _employeesRepository.GetAllAsync(cts);
            if (data == null || data.Count == 0)
            {
                string error = $"The list of {nameof(Employee)} is empty.";
                _logger.LogError(error);
                throw new EmployeeTableIsEmptyException();
            }
            return data;
        }

        public async Task<Employee> GetByIdAsync(Guid id, CancellationToken cts)
        {
            if (await _employeesRepository.GetByIdAsync(id, cts) is not Employee employee)
            {
                string error = $"Couldn`t find the entry of {nameof(Employee)} with the provided Key '{id}'.";
                _logger.LogError(error);
                throw new EmployeeIsNotFoundException();
            }
            return employee;
        }

        public async Task AddAsync(Employee entity, CancellationToken cts)
        {
            await _employeesRepository.AddAsync(entity, cts);
        }

        public async Task UpdateByIdAsync(Guid id, Employee entity, CancellationToken cts)
        {
            if (!await _employeesRepository.Exists(id))
            {
                string error = $"Couldn`t find the entry of {nameof(Employee)} with the provided Key '{id}'.";
                _logger.LogError(error);
                throw new EmployeeIsNotFoundException();
            }
            var rolesCheck = await CheckIfRoleExists(entity.Roles);
            if (!rolesCheck.IsSuccessful)
            {
                //TODO: требуется доработать выбрасываемую ошибку и проверку, так как может быть передано
                var notFoundRolesStr = String.Join(", ", rolesCheck.NotFoundRoles);
                string error = $"Couldn`t find {rolesCheck.NotFoundRoles.Count} enties of '{nameof(Role)}': {notFoundRolesStr}.";
                _logger.LogError(error);
                throw new RoleIsNotFoundException();
            }

            await _employeesRepository.UpdateByIdAsync(id, entity, cts);            
        }

        public async Task DeleteByIdAsync(Guid id, CancellationToken cts)
        {
            if (!await _employeesRepository.Exists(id))
            {
                string error = $"Couldn` find the entry of {nameof(Employee)} with the provided Key '{id}'.";
                _logger.LogError(error);
                throw new EmployeeIsNotFoundException();
            }
            await _employeesRepository.DeleteByIdAsync(id, cts);
        }

        private async Task<(bool IsSuccessful, IList<Guid> NotFoundRoles)> CheckIfRoleExists(IList<Role> roles)
        {
            var result = true;
            List<Guid> notFoundRoles = new();
            foreach (var role in roles)
            {
                if (await _rolesRepository.Exists(role.Id) is false)
                {
                    notFoundRoles.Add(role.Id);
                    result = false;
                }
            }
            return (result, notFoundRoles);
        }
    }
}
