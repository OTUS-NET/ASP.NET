using System;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers.EmployeeMappers;

public class EmployeeMappers(IServiceProvider services) : IEmployeeMappers
{
    /// <inheritdoc />
    public IMapper<Employee, EmployeeResponse> EmployeeToEmployeeResponse { get; }
        = services.GetRequiredService<IMapper<Employee, EmployeeResponse>>();

    /// <inheritdoc />
    public IMapper<Employee, EmployeeShortResponse> EmployeeToEmployeeShortResponse { get; }
        = services.GetRequiredService<IMapper<Employee, EmployeeShortResponse>>();
    
    /// <inheritdoc />
    public IMapper<EmployeeCreationRequest, Employee> EmployeeCreationRequestToEmployee { get; }
        = services.GetRequiredService<IMapper<EmployeeCreationRequest, Employee>>();
    
    /// <inheritdoc />
    public IMapper<EmployeeUpdateRequest, Employee> EmployeeUpdateRequestToEmployee { get; }
        = services.GetRequiredService<IMapper<EmployeeUpdateRequest, Employee>>();
}