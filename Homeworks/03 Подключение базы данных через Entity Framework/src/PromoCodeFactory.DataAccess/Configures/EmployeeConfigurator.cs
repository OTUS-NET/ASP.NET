using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.DataAccess.Entities;

namespace PromoCodeFactory.DataAccess.Configures;

internal class EmployeeConfigurator : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.Property(x => x.FirstName)
            .IsRequired()            
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
           .IsRequired()
           .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => e.RoleId);
    }
}
