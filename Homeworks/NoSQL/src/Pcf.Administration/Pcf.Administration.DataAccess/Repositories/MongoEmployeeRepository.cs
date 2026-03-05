using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Mongo;

namespace Pcf.Administration.DataAccess.Repositories
{
    public class MongoEmployeeRepository : IRepository<Employee>
    {
        private readonly IMongoCollection<Employee> _employees;
        private readonly IMongoCollection<Role> _roles;

        public MongoEmployeeRepository(IMongoDatabase database, IOptions<MongoDbSettings> settings)
        {
            _employees = database.GetCollection<Employee>(settings.Value.EmployeesCollectionName);
            _roles = database.GetCollection<Role>(settings.Value.RolesCollectionName);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var employees = await _employees.Find(FilterDefinition<Employee>.Empty).ToListAsync();
            await HydrateRolesAsync(employees);
            return employees;
        }

        public async Task<Employee> GetByIdAsync(Guid id)
        {
            var employee = await _employees.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (employee == null)
                return null;

            await HydrateRolesAsync(new[] { employee });
            return employee;
        }

        public async Task<IEnumerable<Employee>> GetRangeByIdsAsync(List<Guid> ids)
        {
            var employees = await _employees.Find(x => ids.Contains(x.Id)).ToListAsync();
            await HydrateRolesAsync(employees);
            return employees;
        }

        public async Task<Employee> GetFirstWhere(Expression<Func<Employee, bool>> predicate)
        {
            var employee = await _employees.AsQueryable().FirstOrDefaultAsync(predicate);
            if (employee == null)
                return null;

            await HydrateRolesAsync(new[] { employee });
            return employee;
        }

        public async Task<IEnumerable<Employee>> GetWhere(Expression<Func<Employee, bool>> predicate)
        {
            var employees = await _employees.AsQueryable().Where(predicate).ToListAsync();
            await HydrateRolesAsync(employees);
            return employees;
        }

        public async Task AddAsync(Employee entity)
        {
            await _employees.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Employee entity)
        {
            await _employees.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = false });
        }

        public async Task DeleteAsync(Employee entity)
        {
            await _employees.DeleteOneAsync(x => x.Id == entity.Id);
        }

        private async Task HydrateRolesAsync(IEnumerable<Employee> employees)
        {
            var list = employees as IList<Employee> ?? employees.ToList();
            if (list.Count == 0)
                return;

            var roleIds = list.Select(x => x.RoleId).Distinct().ToList();
            var roles = await _roles.Find(x => roleIds.Contains(x.Id)).ToListAsync();
            var rolesById = roles.ToDictionary(x => x.Id, x => x);

            foreach (var employee in list)
            {
                if (rolesById.TryGetValue(employee.RoleId, out var role))
                    employee.Role = role;
            }
        }
    }
}

