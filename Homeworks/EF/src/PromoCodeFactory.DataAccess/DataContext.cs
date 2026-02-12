using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.DataAccess
{
    public class DataContext : DbContext
    {
        // 4. Настроить маппинг классов Employee, Roles, Customer,Preference и PromoCode на базу данных через EF
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            // 4.
            // PromoCode имеет ссылку на Preference
            modelBuilder.Entity<PromoCode>()
                .HasOne(p => p.Preference)
                .WithMany(p => p.PromoCodes)
                .HasForeignKey(p => p.PreferenceId);

            // Employee имеет ссылку на Role
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany(r => r.Employee)
                .HasForeignKey(e => e.RoleId);

            // Customer имеет набор Preference: Many-to-many через сущность CustomerPreference
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Preferences)
                .WithMany(p => p.Customers)
                .UsingEntity<CustomerPreference>(
                    j => j.HasOne(c => c.Preference)
                        .WithMany()
                        .HasForeignKey(cp => cp.PreferenceId),
                    j => j
                        .HasOne(c => c.Customer)
                        .WithMany()
                        .HasForeignKey(cp => cp.CustomerId),
                    j =>
                    {
                        j.HasKey(cp => new { cp.CustomerId, cp.PreferenceId });
                        j.ToTable("CustomerPreferences");
                    }
                );

            // Связь Customer и Promocode реализовать через One-To-Many
            //modelBuilder.Entity<PromoCode>()
            //    .HasOne(p => p.Customer)
            //    .WithMany(c => c.Promocodes)
            //    .HasForeignKey(p => p.CustomerId)
            //    .OnDelete(DeleteBehavior.Cascade); // 5. при удалении также нужно удалить ранее выданные промокоды клиента


            // 7.Связь Customer и Promocode реализовать через Many-To-Many
            // при удалении также нужно удалить ранее выданные промокоды клиента
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Promocodes)
                .WithMany(p => p.Customers)
                .UsingEntity<Dictionary<Guid, Guid>>(
                    "CustomerPromoCodes",
                    j => j.HasOne<PromoCode>().WithMany().HasForeignKey("PromoCodeId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Customer>().WithMany().HasForeignKey("CustomerId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasKey("CustomerId", "PromoCodeId")
                );

            modelBuilder.Entity<PromoCode>()
                .HasOne(p => p.PartnerManager)
                .WithMany(c => c.PromoCodes)
                .HasForeignKey(p => p.PartnerManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Строковые поля должны иметь ограничения на MaxLength
            modelBuilder.Entity<Role>(r =>
            {
                r.Property(x => x.Name).HasMaxLength(255);
                r.Property(x => x.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<Employee>(e =>
            {
                e.Property(x => x.FirstName).HasMaxLength(255);
                e.Property(x => x.LastName).HasMaxLength(255);
                e.Property(x => x.Email).HasMaxLength(255);
            });

            modelBuilder.Entity<Preference>(p =>
            {
                p.Property(x => x.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Customer>(c =>
            {
                c.Property(x => x.FirstName).HasMaxLength(255);
                c.Property(x => x.LastName).HasMaxLength(255);
                c.Property(x => x.Email).HasMaxLength(255);
            });

            modelBuilder.Entity<PromoCode>(p =>
            {
                p.Property(x => x.Code).HasMaxLength(20);
                p.Property(x => x.ServiceInfo).HasMaxLength(1000);
                p.Property(x => x.PartnerName).HasMaxLength(255);
            });

            // 3. База должна удаляться и создаваться каждый раз, заполняясь тестовыми данными из FakeDataFactory.
            modelBuilder.Entity<Role>().HasData(FakeDataFactory.Roles);
            modelBuilder.Entity<Employee>().HasData(FakeDataFactory.Employees);
            modelBuilder.Entity<Preference>().HasData(FakeDataFactory.Preferences);
            modelBuilder.Entity<Customer>().HasData(FakeDataFactory.Customers
                .Select(x => new Customer
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Preferences = [],
                }));
            modelBuilder.Entity<CustomerPreference>().HasData(
                FakeDataFactory.Customers
                .SelectMany(c => c.Preferences, (c, p) => new CustomerPreference
                {
                    CustomerId = c.Id,
                    PreferenceId = p.Id
                })
            );

            base.OnModelCreating(modelBuilder);

            // Для всех свойств типа Guid настройте преобразование
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(Guid))
                    {
                        property.SetColumnType("uuid");
                    }
                    else if (property.ClrType == typeof(Guid?))
                    {
                        property.SetColumnType("uuid");
                    }
                }
            }
        }
    }
}
