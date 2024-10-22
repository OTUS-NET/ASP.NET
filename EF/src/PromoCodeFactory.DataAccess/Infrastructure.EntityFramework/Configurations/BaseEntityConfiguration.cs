using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Infrastructure.EntityFramework.Configurations;

public static class BaseEntityConfiguration
{
    public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion<DateTime>()
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasConversion<DateTime>()
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);
    }
}