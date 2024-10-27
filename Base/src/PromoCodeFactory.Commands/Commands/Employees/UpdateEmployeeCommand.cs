using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Common.Exceptions;
using PromoCodeFactory.Contracts.Employees;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Commands.Employees;

public class UpdateEmployeeCommand : IRequest
{
    public required Guid Id { get; set; }
    
    public required EmployeeSetDto Data { get; set; }
}

public class UpdateEmployeeCommandHandler(PromoCodesDbContext dbContext) : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees
            .Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException("Employee not found");
        }
        
        if (await _dbContext.Employees.AnyAsync(
                x => x.Email == request.Data.Email.ToLower(),
                cancellationToken) &&
            !employee.Email.ToLower().Equals(request.Data.Email, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new ArgumentException("Employee with this email already exists");
        }

        employee.FirstName = request.Data.FirstName;
        employee.LastName = request.Data.LastName;
        employee.Email = request.Data.Email;

        if (request.Data.Role is not null)
        {
            employee.Role.Name = request.Data.Role.Name;
            employee.Role.Description = request.Data.Role.Description;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}