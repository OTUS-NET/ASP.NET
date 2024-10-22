using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Infrastructure.EntityFramework.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        BaseEntityConfiguration.ConfigureBaseEntity(builder);
        
        builder.Property(x => x.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.AppliedPromoCodesCount)
            .HasColumnName("applied_promo_codes_count")
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Employee
            {
                Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                Email = "sansanich@somemail.ru",
                FirstName = "Сан",
                LastName = "Саныч",
                AppliedPromoCodesCount = 5,
                RoleId = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02")
            },
            new Employee
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "marivanna@somemail.ru",
                FirstName = "Мария",
                LastName = "Ивановна",
                AppliedPromoCodesCount = 10,
                RoleId = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665")
            }
        );
    }
}