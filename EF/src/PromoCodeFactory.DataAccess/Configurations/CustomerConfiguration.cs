using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations
{
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(x => x.FirstName).HasMaxLength(25).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(25).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(30).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();

            builder.HasData(FakeDataFactory.Customers);
        }
    }
}
