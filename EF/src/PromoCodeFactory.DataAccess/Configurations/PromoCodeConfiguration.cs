using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations
{
    internal class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.Property(x => x.PartnerName).HasMaxLength(25);
            builder.Property(x => x.ServiceInfo).HasMaxLength(250);
            builder.HasIndex(x => x.CustomerId);

            builder.HasOne(p => p.Customer).WithMany(c => c.PromoCodes).HasForeignKey(p => p.CustomerId);
        }
    }
}
