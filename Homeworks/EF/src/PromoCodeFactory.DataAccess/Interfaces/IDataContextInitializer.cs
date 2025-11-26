using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Interfaces
{
    public interface IDataContextInitializer
    {
        void Seed();

        Task SeedAsync(CancellationToken ct);
    }
}
