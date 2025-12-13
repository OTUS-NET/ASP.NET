using PromoCodeFactory.Core.Abstractions.Models.Employees;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Repositories.Interfaces.Employees;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EmployeeRepository : InMemoryRepository<Employee>, IEmployeeRepository
    {
        private IRepository<Role> roleRepository;

        public EmployeeRepository(IEnumerable<Employee> data, IRepository<Role> roleRepository) : base(data)
        {
            this.roleRepository = roleRepository;
        }

        public async Task<Employee> AddAsync(EmployeeCreateDto entity, CancellationToken cancellationToken)
        {
            var roles = new List<Role>();

            foreach (var rol in entity.RoleIds)
                roles.Add(await this.roleRepository.GetByIdAsync(rol));

            var newEmployee = new Employee()
            {
                Id = Guid.NewGuid(),
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Roles = roles,
                AppliedPromocodesCount = 0
            };

            ((IList<Employee>)this.Data).Add(newEmployee);

            return newEmployee;
        }

        public Task<Guid> DeleteAsync(Guid entityId, CancellationToken cancellationToken)
        {
            var employee = this.Data.Where(emp => emp.Id == entityId).FirstOrDefault();

            if(employee != null)
            {
                ((IList<Employee>)this.Data).Remove(employee);

                return Task.FromResult(employee.Id);
            }

            throw new NotFoundEntityException(nameof(Employee));
        }

        public async Task<Employee> UpdateAsync(Guid entityId, EmployeeUpdateDto entity, CancellationToken cancellationToken)
        {
            var emp = await this.GetByIdAsync(entityId);

            emp.FirstName = entity.FirstName;
            emp.LastName = entity.LastName;
            emp.Email = entity.Email;
            emp.AppliedPromocodesCount = entity.AppliedPromocodesCount;

            if (entity.RoleIds == null)
                return emp;

            var roles = new List<Role>();

            foreach (var rol in entity.RoleIds)
                roles.Add(await this.roleRepository.GetByIdAsync(rol));

            emp.Roles = roles;

            return emp;
        }
    }
}
