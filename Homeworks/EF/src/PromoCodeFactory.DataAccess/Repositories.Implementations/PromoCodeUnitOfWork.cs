using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.DataAccess.Repositories.Implementations;

public class PromoCodeUnitOfWork : IPromoCodeUnitOfWork
{
    private readonly DatabaseContext _databaseContext;
    public IPromoCodeRepository PromoCodeRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IPreferenceRepository PreferenceRepository { get; }
    public IEmployeeRepository EmployeeRepository { get; }

    public PromoCodeUnitOfWork(
        DatabaseContext databaseContext, 
        IPromoCodeRepository promoCodeRepository, 
        ICustomerRepository customerRepository, 
        IPreferenceRepository preferenceRepository, 
        IEmployeeRepository employeeRepository)
    {
        _databaseContext = databaseContext;
        PromoCodeRepository = promoCodeRepository;
        CustomerRepository = customerRepository;
        PreferenceRepository = preferenceRepository;
        EmployeeRepository = employeeRepository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _databaseContext.SaveChangesAsync(cancellationToken);
    }
}