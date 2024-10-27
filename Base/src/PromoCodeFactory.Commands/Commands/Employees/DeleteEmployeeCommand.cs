using MediatR;
using PromoCodeFactory.Common.Exceptions;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Commands.Employees;

public class DeleteEmployeeCommand : IRequest
{
    public required Guid Id { get; set; }
}

public class DeleteEmployeeCommandHandler(PromoCodesDbContext dbContext) : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees.FindAsync([request.Id], cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException("Employee not found");
        }

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}