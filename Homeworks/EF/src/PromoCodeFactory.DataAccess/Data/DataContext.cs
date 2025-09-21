using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureRole(modelBuilder);
            ConfigureEmployee(modelBuilder);
            ConfigureCustomer(modelBuilder);
            ConfigurePreference(modelBuilder);
            ConfigurePromoCode(modelBuilder);
            ConfigureCustomerPreference(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureRole(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.Description)
                    .HasMaxLength(200);

                entity.HasIndex(r => r.Name)
                    .IsUnique();
            });
        }
        private void ConfigureEmployee(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired();

                entity.HasOne(e => e.Role).
                       WithMany(e => e.Emploees).
                       HasForeignKey(e => e.RoleId)
                           .OnDelete(DeleteBehavior.Restrict);

                entity.Ignore(e => e.FullName);

                entity.HasIndex(e => e.Email)
                    .IsUnique();
            });
        }

        private void ConfigureCustomer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Ignore(c => c.FullName);

                entity.HasIndex(e => e.Email)
                    .IsUnique();
            });
        }

        private void ConfigurePreference(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Preference>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }

        private void ConfigurePromoCode(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.HasKey(pc => pc.Id);

                entity.Property(pc => pc.Code)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(pc => pc.ServiceInfo)
                    .HasMaxLength(100);

                entity.Property(pc => pc.PartnerName)
                    .IsRequired()
                    .HasMaxLength(100);

                // удяляем каскадно (пункт 5 из ДЗ)
                entity.HasOne(pc => pc.Customer)
                    .WithMany(c => c.PromoCodes)
                    .HasForeignKey(pc => pc.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);


                entity.HasOne(pc => pc.Preference)
                    .WithMany(p => p.PromoCodes)
                    .HasForeignKey(pc => pc.PreferenceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pc => pc.PartnerManager)
                    .WithMany(e => e.PromoCodes)
                    .HasForeignKey(pc => pc.PartnerManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
        private void ConfigureCustomerPreference(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerPreference>(entity =>
            {
                entity.HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

                entity.HasOne(cp => cp.Customer)
                    .WithMany(c => c.CustomerPreferences)
                    .HasForeignKey(cp => cp.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.Preference)
                    .WithMany(p => p.CustomerPreferences)
                    .HasForeignKey(cp => cp.PreferenceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

    }

}
