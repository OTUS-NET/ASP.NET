using DirectoryOfPreferences.Infrastructure.Repositories.Implementations.EntityFramework;

namespace DirectoryOfPreferences.Infrastructure.Repositories.Implementations
{
    public class PreferenceRepository(ApplicationDbContext context) : EFRepository(context)
    {
    }
}
