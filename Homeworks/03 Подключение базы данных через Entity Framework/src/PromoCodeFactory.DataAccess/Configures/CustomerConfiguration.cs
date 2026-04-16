using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.DataAccess.Entities;

namespace PromoCodeFactory.DataAccess.Configures;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasMany(e => e.Preferences)
            .WithMany()
            .UsingEntity("CustomerPreferences");

        builder.HasMany(e => e.CustomerPromoCodes)
            .WithOne()
            .HasForeignKey(x => x.CustomerId);
    }
}
