using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Common.Exceptions;
using PromoCodeFactory.Common.Extensions;
using PromoCodeFactory.Contracts.Customers;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Queries.Customers;

public class GetCustomerByIdQuery : IRequest<CustomerResponseDto>
{
    public required Guid Id { get; set; }
}

public class GetCustomerByIdQueryHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<GetCustomerByIdQuery, CustomerResponseDto>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<CustomerResponseDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
                           .AsNoTracking()
                           .Include(x => x.CustomerPreferences)
                           .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                       ?? throw new NotFoundException("Customer not found");

        return customer.MapToCustomerResponseDto();
    }
}