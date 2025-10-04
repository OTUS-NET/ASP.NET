using Microsoft.EntityFrameworkCore;
using Pcf.PreferencesDirectory.Core.Domain;

namespace Pcf.PreferencesDirectory.DataAccess
{
    public class DataContext
        : DbContext
    {
        public DbSet<Preference> Preferences { get; set; }

        public DataContext()
        {

        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }
    }
}
