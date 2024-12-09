using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerPreference> CustomerPreferences { get; set; }

        public DbSet<Preference> Preferences { get; set; }

        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PromoCode>()
                        .HasOne(p => p.Customer)
                        .WithMany(c => c.PromoCodes)
                        .HasForeignKey(p => p.CustomerId);

            modelBuilder.Entity<CustomerPreference>()
                        .HasKey(cp => new { cp.CustomerId, cp.PreferenceId }); 

            modelBuilder.Entity<CustomerPreference>()
                        .HasOne(cp => cp.Customer)
                        .WithMany(c => c.CustomerPreferences)  
                        .HasForeignKey(cp => cp.CustomerId);

            modelBuilder.Entity<CustomerPreference>()
                        .HasOne(cp => cp.Preference)
                        .WithMany(p => p.CustomerPreferences)  
                        .HasForeignKey(cp => cp.PreferenceId);

            modelBuilder.Entity<Customer>().Property(c => c.FirstName).HasMaxLength(30);
            modelBuilder.Entity<Customer>().Property(c => c.LastName).HasMaxLength(30);
            modelBuilder.Entity<Customer>().Property(c => c.Email).HasMaxLength(254);

            modelBuilder.Entity<Preference>().Property(p => p.Name).HasMaxLength(30);

            modelBuilder.Entity<PromoCode>().Property(pm => pm.Code).HasMaxLength(15);
            modelBuilder.Entity<PromoCode>().Property(pm => pm.ServiceInfo).HasMaxLength(30);
            modelBuilder.Entity<PromoCode>().Property(pm => pm.PartnerName).HasMaxLength(30);

            modelBuilder.Entity<Employee>().Property(e => e.FirstName).HasMaxLength(30);
            modelBuilder.Entity<Employee>().Property(e => e.LastName).HasMaxLength(30);
            modelBuilder.Entity<Employee>().Property(e => e.Email).HasMaxLength(254);

            modelBuilder.Entity<Role>().Property(r => r.Name).HasMaxLength(20);
            modelBuilder.Entity<Role>().Property(r => r.Description).HasMaxLength(100);
        }
    }
}