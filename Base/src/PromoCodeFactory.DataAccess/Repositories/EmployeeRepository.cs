using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.Dtos.Administation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories;

public sealed class EmployeeRepository(IEnumerable<Employee> data, IRepository<Role> roleRepository)
    : InMemoryRepository<Employee>(data), IEmployeeRepository
{
    public async Task<Employee> AddAsync(EmployeeDto dto)
    {
        var newEntity = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
        };
        await FillRoles(dto, newEntity);

        Data.Add(newEntity);

        return newEntity;
    }

    public async Task UpdateAsync(Guid id, EmployeeDto dto)
    {
        var dbEntity = Data.FirstOrDefault(x => x.Id == id)
            ?? throw new ArgumentOutOfRangeException($"Can't find an entity with id: {id} in {nameof(Employee)} repository");

        dbEntity.FirstName = dto.FirstName;
        dbEntity.LastName = dto.LastName;
        dbEntity.Email = dto.Email;

        await FillRoles(dto, dbEntity);
    }

    private async Task FillRoles(EmployeeDto dto, Employee employee)
    {
        employee.Roles = [];

        foreach (var roleId in dto.Roles)
        {
            var role = await roleRepository.GetByIdAsync(roleId)
                ?? throw new ArgumentOutOfRangeException($"Can't find an entity with id: {roleId} in {nameof(Role)} repository"); ;
            employee.Roles.Add(role);
        }
    }
}
