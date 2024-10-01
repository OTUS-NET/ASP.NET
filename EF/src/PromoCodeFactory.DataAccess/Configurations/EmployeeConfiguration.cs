using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations
{
    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(e => e.FirstName).HasMaxLength(25).IsRequired();
            builder.Property(e => e.LastName).HasMaxLength(25).IsRequired();
            builder.Property(e => e.Email).HasMaxLength(30).IsRequired();
            builder.HasIndex(e => e.Email).IsUnique();

            builder.HasOne(e => e.Role).WithMany(r => r.Employees).HasForeignKey(e => e.RoleId);
            builder.HasData(FakeDataFactory.Employees);
        }
    }
}
