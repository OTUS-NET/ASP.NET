using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Data
{
    public class SQLiteDatabaseContext : DbContext
    {
        public SQLiteDatabaseContext(DbContextOptions<SQLiteDatabaseContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(build =>
            {
                build.Property(x => x.Name).HasMaxLength(50);
                build.Property(x => x.Description).HasMaxLength(100);
                build.HasData(FakeDataFactory.Roles);
            });
            modelBuilder.Entity<Employee>(build =>
            {
                build.Property(x => x.FirstName).HasMaxLength(50);
                build.Property(x => x.LastName).HasMaxLength(50);
                build.Property(x => x.Email).HasMaxLength(100);
                build.HasData(FakeDataFactory.Employees);
            });
            modelBuilder.Entity<Customer>(build =>
            {
                build.Property(x => x.FirstName).HasMaxLength(50);
                build.Property(x => x.LastName).HasMaxLength(50);
                build.Property(x => x.Email).HasMaxLength(100);
                build.HasData(FakeDataFactory.Customers);
                build.HasMany(x => x.Preferences).WithMany(x => x.Customers).UsingEntity<CustomerPreference>(x =>
                {
                    x.HasOne(y => y.Customer).WithMany(y => y.CustomerPreferences).HasForeignKey(y => y.CustomerId);
                    x.HasOne(y => y.Preference).WithMany(y => y.CustomerPreferences).HasForeignKey(y => y.PreferenceId);
                    x.HasData(FakeDataFactory.CustomerPreferences);
                });
                build.HasMany(x => x.PromoCodes);
            });
            modelBuilder.Entity<Preference>(build =>
            {
                build.Property(x => x.Name).HasMaxLength(50);
                build.HasData(FakeDataFactory.Preferences);
            });
        }
    }
}