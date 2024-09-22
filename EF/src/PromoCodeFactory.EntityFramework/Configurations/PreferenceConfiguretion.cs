using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.EntityFramework.Configurations
{
    public class PreferenceConfiguretion : IEntityTypeConfiguration<Preference>
    {
        public void Configure(EntityTypeBuilder<Preference> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).HasMaxLength(100);
            builder.HasMany(p => p.PromoCodes)
                .WithOne(c => c.Preference)
                .HasForeignKey(c => c.PreferenceId);
            builder.HasMany(p => p.Customers)
                .WithOne(c => c.Preference)
                .HasForeignKey(c => c.PreferenceId);
        }
    }
}
