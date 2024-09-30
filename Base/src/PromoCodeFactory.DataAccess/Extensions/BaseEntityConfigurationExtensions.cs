using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Extensions;

public static class BaseEntityConfigurationExtensions
{
    public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasConversion<DateTime>()
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);
        
        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasConversion<DateTime>()
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);
    }
}