using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(25).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(250);
            builder.HasMany(x => x.Employees).WithOne(r => r.Role).HasForeignKey(x => x.RoleId);
            builder.HasData(FakeDataFactory.Roles);
        }
    }
}
