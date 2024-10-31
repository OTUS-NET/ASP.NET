using Microsoft.EntityFrameworkCore;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.DataAccess
{
    public class DataContext
        : DbContext
    {

        public DbSet<Partner> Partners { get; set; }

        public DataContext()
        {

        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}