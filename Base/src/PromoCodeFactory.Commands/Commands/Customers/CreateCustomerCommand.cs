using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Contracts;
using PromoCodeFactory.Contracts.Customers;
using PromoCodeFactory.Core.PromoCodeManagement;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Commands.Customers;

public class CreateCustomerCommand : IRequest<ResponseId<Guid>>
{
    public required CustomerSetDto Data { get; set; }
}

public class CreateCustomerCommandHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<CreateCustomerCommand, ResponseId<Guid>>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<ResponseId<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existedCustomer = await _dbContext.Customers
            .AnyAsync(x => x.Email == request.Data.Email.ToLower(),
            cancellationToken);

        if (existedCustomer)
        {
            throw new ArgumentException("Customer with this email already exists");
        }

        var customer = await _dbContext.Customers.AddAsync(new Customer
        {
            FirstName = request.Data.FirstName,
            LastName = request.Data.LastName,
            Email = request.Data.Email.ToLower(),
        }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new ResponseId<Guid>
        {
            Id = customer.Entity.Id
        };
    }
}