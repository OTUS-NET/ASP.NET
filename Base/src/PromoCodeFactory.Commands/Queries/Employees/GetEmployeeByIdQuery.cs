using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Common.Exceptions;
using PromoCodeFactory.Contracts.Employees;
using PromoCodeFactory.Contracts.Roles;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Queries.Employees;

public class GetEmployeeByIdQuery : IRequest<EmployeeResponseDto>
{
    public required Guid Id { get; set; }
}

public class GetEmployeeByIdQueryHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponseDto>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<EmployeeResponseDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees
            .AsNoTracking()
            .Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException("Employee not found");
        }

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            Email = employee.Email,
            FullName = employee.FullName,
            Roles = 
            [
                new RoleItemResponse
                {
                    Id = employee.Role.Id,
                    Name = employee.Role.Name,
                    Description = employee.Role.Description,
                }
            ],
            AppliedPromocodesCount = employee.AppliedPromoCodesCount
        };
    }
}