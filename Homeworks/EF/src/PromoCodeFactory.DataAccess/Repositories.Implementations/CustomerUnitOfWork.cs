using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.DataAccess.Repositories.Implementations;

public class CustomerUnitOfWork : ICustomerUnitOfWork
{
    private readonly DatabaseContext _databaseContext;
    public ICustomerRepository CustomerRepository { get; }
    public IPreferenceRepository PreferenceRepository { get; }

    public CustomerUnitOfWork(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        CustomerRepository = new CustomerRepository(databaseContext);
        PreferenceRepository = new PreferenceRepository(databaseContext);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _databaseContext.SaveChangesAsync(cancellationToken);
    }
}