using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Services;
using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Services;

public class EmployeesService(IRepository<Employee> _repository) : IEmployeesService
{
    public async Task<Employee> GetById(Guid id, CancellationToken ct)
    {
        return await _repository.GetById(id, ct);
    }

    public Task CreateEmployee(Employee request, CancellationToken ct)
    {
        return _repository.Add(request, ct);
    }

    public Task UpdateEmployee(Employee request, CancellationToken ct)
    {
        return _repository.Update(request, ct);
    }
}
