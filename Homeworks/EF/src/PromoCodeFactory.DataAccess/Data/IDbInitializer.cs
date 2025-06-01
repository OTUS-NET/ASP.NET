using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Data
{
    public interface IDbInitializer
    {
        public Task InitializeDbAsync();
    }
}
