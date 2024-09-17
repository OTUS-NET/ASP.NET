using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IMongoCollection<Employee> _employees;

    public EmployeeRepository(MongoDbContext context)
    {
        _employees = context.Employees;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _employees.Find(emp => true).ToListAsync();
    }

    public async Task<Employee> GetEmployeeByIdAsync(string id)
    {
        // Преобразуем строку в Guid
        if (Guid.TryParse(id, out Guid employeeId))
        {
            return await _employees.Find(emp => emp.Id == employeeId).FirstOrDefaultAsync();
        }

        // Вернем null или можно выбросить исключение
        return null;
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        await _employees.InsertOneAsync(employee);
    }

    public async Task UpdateEmployeeAsync(string id, Employee updatedEmployee)
    {
        // Преобразуем строку в Guid
        if (Guid.TryParse(id, out Guid employeeId))
        {
            await _employees.ReplaceOneAsync(emp => emp.Id == employeeId, updatedEmployee);
        }
    }

    public async Task DeleteEmployeeAsync(string id)
    {
        // Преобразуем строку в Guid
        if (Guid.TryParse(id, out Guid employeeId))
        {
            await _employees.DeleteOneAsync(emp => emp.Id == employeeId);
        }
    }
}
