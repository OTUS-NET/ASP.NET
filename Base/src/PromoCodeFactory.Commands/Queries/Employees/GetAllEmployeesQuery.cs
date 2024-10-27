using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Contracts.Employees;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Queries.Employees;

public class GetAllEmployeesQuery : IRequest<List<EmployeeShortResponse>>
{
}

public class GetAllEmployeeQueryHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<GetAllEmployeesQuery, List<EmployeeShortResponse>>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<List<EmployeeShortResponse>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _dbContext.Employees
            .AsNoTracking()
            .Select(x => new EmployeeShortResponse
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName,
            })
            .ToListAsync(cancellationToken);

        return employees;
    }
}