using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public const string DefaultDefaultSchema = "PROMO";
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        public DataContext()
        {
            
        }

        public DataContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultDefaultSchema);

            modelBuilder.Entity<CustomerPreference>().HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

            modelBuilder.Entity<CustomerPreference>()
                .HasOne(cp => cp.Customer)
                .WithMany(c => c.CustomerPreferences)
                .HasForeignKey(cp => cp.CustomerId);

            modelBuilder.Entity<CustomerPreference>()
                .HasOne(cp => cp.Preference);

            modelBuilder.Entity<Customer>()
                .HasMany(cp => cp.PromoCodes)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>().Property(p => p.FirstName).HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(p => p.LastName).HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(p => p.Email).HasMaxLength(200);
            modelBuilder.Entity<Customer>().Ignore(p => p.FullName);

            modelBuilder.Entity<Preference>().Property(p => p.Name).HasMaxLength(100);

            modelBuilder.Entity<PromoCode>().HasOne(p => p.Preference).WithMany().HasForeignKey(p => p.PreferenceId);
            modelBuilder.Entity<PromoCode>().HasOne(p => p.PartnerManager).WithMany().HasForeignKey(p => p.ManagerId);
            modelBuilder.Entity<PromoCode>().Property(p => p.Code).HasMaxLength(50);
            modelBuilder.Entity<PromoCode>().Property(p => p.ServiceInfo).HasMaxLength(100);
            modelBuilder.Entity<PromoCode>().Property(p => p.PartnerName).HasMaxLength(200);

            modelBuilder.Entity<Role>().Property(p => p.Name).HasMaxLength(100);
            modelBuilder.Entity<Role>().Property(p => p.Description).HasMaxLength(200);

            modelBuilder.Entity<Employee>().HasOne(e => e.Role);
            modelBuilder.Entity<Employee>().Property(e => e.Email).HasMaxLength(100);
            modelBuilder.Entity<Employee>().Property(e => e.FirstName).HasMaxLength(100);
            modelBuilder.Entity<Employee>().Property(e => e.LastName).HasMaxLength(200);
            modelBuilder.Entity<Employee>().Ignore(e => e.FullName);

            base.OnModelCreating(modelBuilder);
        }
    }
}
