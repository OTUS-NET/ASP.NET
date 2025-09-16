using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers;

public interface IEmployeeMappers
{
    IMapper<Employee, EmployeeResponse> EmployeeToEmployeeResponse { get; }
    IMapper<Employee, EmployeeShortResponse> EmployeeToEmployeeShortResponse { get; }
    IMapper<EmployeeCreationRequest, Employee> EmployeeCreationRequestToEmployee { get; }
    IMapper<EmployeeUpdateRequest, Employee> EmployeeUpdateRequestToEmployee { get; }
}