using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    private const string TABLE_NAME = "employees";

    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id).HasName($"PK_{TABLE_NAME}");
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();

        builder.Property(u => u.FirstName).IsRequired().HasColumnName("first_name");
        builder.Property(u => u.LastName).IsRequired().HasColumnName("last_name");
        builder.Property(u => u.RoleId).IsRequired().HasColumnName("role_id");
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.AppliedPromocodesCount).IsRequired().HasDefaultValue(0);

        builder.HasOne(e => e.Role).WithMany(r => r.Employees).HasForeignKey(r => r.RoleId).IsRequired();
    }
}

