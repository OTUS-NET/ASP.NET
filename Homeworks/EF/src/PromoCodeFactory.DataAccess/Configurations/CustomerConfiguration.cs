using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    private const string TABLE_NAME = "customers";

    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(e => e.Id).HasName($"PK_{TABLE_NAME}");
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();

        builder.Property(u => u.FirstName).IsRequired().HasColumnName("first_name");
        builder.Property(u => u.LastName).IsRequired().HasColumnName("last_name");
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);

        builder.HasMany(c => c.CustomerPreferences)
               .WithOne(cp => cp.Customer)
               .HasForeignKey(cp => cp.CustomerId);
    }
}

