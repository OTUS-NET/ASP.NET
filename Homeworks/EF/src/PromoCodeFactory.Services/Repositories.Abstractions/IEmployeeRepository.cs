using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Services.Contracts.Employee;

namespace PromoCodeFactory.Services.Repositories.Abstractions;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync(
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        EmployeeFilterDto? employeeFilterDto = null);

    Task AddRangeIfNotExistsAsync(IEnumerable<Employee> employees, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetAsync(Guid id, CancellationToken cancellationToken);
}