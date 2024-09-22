using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.EntityFramework.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(100);
            builder.HasIndex(x => x.Name).IsUnique(); 
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.HasMany(r => r.Employees)
                .WithOne(e => e.Role)
                .HasForeignKey(t => t.RoleId);  
        }
    }
}
