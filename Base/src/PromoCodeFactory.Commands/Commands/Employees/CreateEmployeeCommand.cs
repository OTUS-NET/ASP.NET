using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Contracts;
using PromoCodeFactory.Contracts.Employees;
using PromoCodeFactory.Core.Administration;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Commands.Employees;

public class CreateEmployeeCommand : IRequest<ResponseId<Guid>>
{
    public required EmployeeSetDto Data { get; set; }
}

public class CreateEmployeeCommandHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<CreateEmployeeCommand, ResponseId<Guid>>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<ResponseId<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existedEmployee = await _dbContext.Employees
            .AnyAsync(x => x.Email == request.Data.Email.ToLower(),
                cancellationToken);

        if (existedEmployee)
        {
            throw new ArgumentException("Employee with this email already exists");
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Email = request.Data.Email,
            FirstName = request.Data.FirstName,
            LastName = request.Data.LastName,
            Role = new Role()
        };

        if (request.Data.Role is not null)
        {
            employee.Role.Name = request.Data.Role.Name;
            employee.Role.Description = request.Data.Role.Description;
        }

        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ResponseId<Guid>
        {
            Id = employee.Id
        };
    }
}