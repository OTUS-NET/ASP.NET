using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Infrastructure.EntityFramework.Configurations;

public class CustomerConfiguration  : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        BaseEntityConfiguration.ConfigureBaseEntity(builder);

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(x => x.CustomerPreferences)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId);
        builder.HasData(
            new Customer
            {
                Id = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                Email = "ivan_sergeev@mail.ru",
                FirstName = "Иван",
                LastName = "Петров"
            },
            new Customer
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "marivanna@somemail.ru",
                FirstName = "Мария",
                LastName = "Ивановна"
            }
        );
    }
}