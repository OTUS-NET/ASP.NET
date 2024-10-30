using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Common.Extensions;
using PromoCodeFactory.Contracts.Customers;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Queries.Customers;

public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerResponseDto>>
{
}

public class GetAllCustomersQueryHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerResponseDto>>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<IEnumerable<CustomerResponseDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _dbContext.Customers
            .AsNoTracking()
            .Include(x => x.CustomerPreferences)
            .ToListAsync(cancellationToken: cancellationToken);

        return customers.Select(x => x.MapToCustomerResponseDto());
    }
}