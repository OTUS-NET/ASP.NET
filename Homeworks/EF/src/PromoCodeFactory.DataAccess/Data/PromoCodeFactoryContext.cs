using System;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public class PromoCodeFactoryContext : DbContext
    {
        public PromoCodeFactoryContext(DbContextOptions<PromoCodeFactoryContext> options)
            : base(options)
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
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(300)
                    .IsRequired();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Ignore(x => x.FullName);

                entity.Property(x => x.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.HasOne(x => x.Role)
                    .WithMany()
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Preference>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Ignore(x => x.FullName);

                entity.Property(x => x.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasMaxLength(200)
                    .IsRequired();
            });

            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ServiceInfo)
                    .HasMaxLength(300)
                    .IsRequired();

                entity.Property(x => x.PartnerName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.HasOne(x => x.Preference)
                    .WithMany(x => x.PromoCodes)
                    .HasForeignKey(x => x.PreferenceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.PartnerManager)
                    .WithMany()
                    .HasForeignKey(x => x.PartnerManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Customer)
                    .WithMany(x => x.PromoCodes)
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CustomerPreference>(entity =>
            {
                entity.HasKey(x => new { x.CustomerId, x.PreferenceId });

                entity.HasOne(x => x.Customer)
                    .WithMany(x => x.CustomerPreferences)
                    .HasForeignKey(x => x.CustomerId);

                entity.HasOne(x => x.Preference)
                    .WithMany(x => x.CustomerPreferences)
                    .HasForeignKey(x => x.PreferenceId);
            });
        }
    }
}