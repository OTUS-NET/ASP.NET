using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Ignore(e => e.FullName);

            builder.Property(x => x.FirstName).HasMaxLength(200);
            builder.Property(x => x.LastName).HasMaxLength(200);
            builder.Property(x => x.Email).HasMaxLength(100);
        }
    }

}
