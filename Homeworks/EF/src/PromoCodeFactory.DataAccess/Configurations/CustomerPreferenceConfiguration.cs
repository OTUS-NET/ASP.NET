using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations
{
    public class CustomerPreferenceConfiguration : IEntityTypeConfiguration<CustomerPreference>
    {
        public void Configure(EntityTypeBuilder<CustomerPreference> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .HasIndex(x => new { x.CustomerId, x.PreferenceId })
                .IsUnique();

            builder.HasOne(cp => cp.Customer)
                   .WithMany(c => c.Preferences)
                   .HasForeignKey(cp => cp.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.Preference)
                   .WithMany(p => p.Customers)
                   .HasForeignKey(cp => cp.PreferenceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
