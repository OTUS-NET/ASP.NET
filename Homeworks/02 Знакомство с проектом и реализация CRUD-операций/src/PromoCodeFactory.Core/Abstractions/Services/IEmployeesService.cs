using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Abstractions.Services;

public interface IEmployeesService
{
    Task<Employee?> GetById(Guid id, CancellationToken ct);
    Task CreateEmployee(Employee request, CancellationToken ct);
    Task UpdateEmployee(Employee request, CancellationToken ct);
    Task DeleteEmployee(Guid id, CancellationToken ct);
}
