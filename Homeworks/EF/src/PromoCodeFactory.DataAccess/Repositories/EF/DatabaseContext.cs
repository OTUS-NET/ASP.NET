using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Domain.PromoCodeManagement.Bindings;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Repositories.EF
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }


        //Выполняется один раз перед ппервым обращением к БД
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Маппинг
            MapEntities(modelBuilder);

            //Заполнение тестовыми данными
            SeedWithTestData(modelBuilder);
        }

        private void MapEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasKey(nameof(BaseEntity.Id));
            modelBuilder.Entity<Role>().Property(r => r.Name).HasMaxLength(100);
            modelBuilder.Entity<Role>().Property(r => r.Description).HasMaxLength(200);

            modelBuilder.Entity<Employee>().HasKey(nameof(BaseEntity.Id));
            modelBuilder.Entity<Employee>().HasOne<Role>(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.LastName).HasMaxLength(100).IsRequired();
            
            modelBuilder.Entity<Preference>().HasKey(nameof(BaseEntity.Id));
            modelBuilder.Entity<Preference>().Property(r => r.Name).HasMaxLength(100);

            modelBuilder.Entity<Customer>().Property(r => r.Email).HasMaxLength(200);
            modelBuilder.Entity<Customer>().Property(r => r.FirstName).HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(r => r.LastName).HasMaxLength(100);

            //many to many
            modelBuilder.Entity<Customer>().HasMany(x => x.Preferences).WithMany()
                .UsingEntity<CustomerPreference>(
                    l => l.HasOne(x => x.Preference).WithMany().HasForeignKey(x => x.PreferenceId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade),
                    l => l.HasKey(nameof(CustomerPreference.PreferenceId), nameof(CustomerPreference.CustomerId))
                    );

            modelBuilder.Entity<PromoCode>().HasKey(nameof(BaseEntity.Id));
            modelBuilder.Entity<PromoCode>().HasOne(x => x.PartnerManager).WithMany().HasForeignKey(x => x.PartnerManagerId).IsRequired();
            modelBuilder.Entity<PromoCode>().HasOne(x => x.Preference).WithMany().HasForeignKey(x => x.PreferenceId).IsRequired();
            modelBuilder.Entity<PromoCode>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Promocodes)
                .HasForeignKey(x => x.CustomerId)
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PromoCode>().Property(x => x.Code).HasMaxLength(100);
            modelBuilder.Entity<PromoCode>().Property(x => x.ServiceInfo).HasMaxLength(100);
        }

        private void SeedWithTestData(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Role>().HasData(FakeDataFactory.Roles);
            modelBuilder.Entity<Preference>().HasData(FakeDataFactory.Preferences);
            modelBuilder.Entity<Customer>().HasData(FakeDataFactory.Customers);
            modelBuilder.Entity<Employee>().HasData(FakeDataFactory.Employees);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
