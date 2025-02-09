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
    public class CustomerPreferenceConfiguration : IEntityTypeConfiguration<CustomerPreference>
    {
        public void Configure(EntityTypeBuilder<CustomerPreference> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
