using System;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.UnitTests.Helpers;

public static class EntityHelper
{
    public static Partner GetPartner(Action<Partner> modifier = null)
    {
        var partner = new Partner
        {
            Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
            Name = "Суперигрушки",
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>
            {
                new()
                {
                    Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                    CreateDate = new DateTime(2020, 07, 9),
                    EndDate = new DateTime(2020, 10, 9),
                    Limit = 100
                }
            }
        };
        
        modifier?.Invoke(partner);
        return partner;
    }
}