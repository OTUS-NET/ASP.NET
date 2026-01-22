using System;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.Builders;

namespace PromoCodeFactory.UnitTests;

public static class TestDataFactory
{
    public static Partner CreateActivePartner(int issuedPromoCodes = 0)
    {
        return new PartnerBuilder()
            .SetActive(true)
            .WithIssuedPromoCodes(issuedPromoCodes)
            .Build();
    }

    public static Partner CreateInactivePartner()
    {
        return new PartnerBuilder()
            .SetActive(false)
            .WithIssuedPromoCodes(5)
            .Build();
    }

    public static Partner CreatePartnerWithActiveLimit(int limitValue = 100, int issuedPromoCodes = 10)
    {
        return new PartnerBuilder()
            .SetActive(true)
            .WithIssuedPromoCodes(issuedPromoCodes)
            .WithActiveLimit(limitValue)
            .Build();
    }

    public static Partner CreatePartnerWithExpiredLimit(int limitValue = 100, int issuedPromoCodes = 10)
    {
        return new PartnerBuilder()
            .SetActive(true)
            .WithIssuedPromoCodes(issuedPromoCodes)
            .WithExpiredLimit(limitValue)
            .Build();
    }

    public static Partner CreatePartnerWithCanceledLimit(int limitValue = 100, int issuedPromoCodes = 10)
    {
        return new PartnerBuilder()
            .SetActive(true)
            .WithIssuedPromoCodes(issuedPromoCodes)
            .WithCanceledLimit(limitValue)
            .Build();
    }

    public static Partner CreatePartnerWithMultipleLimits()
    {
        var partner = new PartnerBuilder()
            .SetActive(true)
            .WithIssuedPromoCodes(15)
            .Build();

        var activeLimit = new PartnerPromoCodeLimitBuilder()
            .WithPartnerId(partner.Id)
            .WithLimit(50)
            .WithEndDate(DateTime.UtcNow.AddDays(30))
            .Build();

        var canceledLimit = new PartnerPromoCodeLimitBuilder()
            .WithPartnerId(partner.Id)
            .WithLimit(100)
            .WithCancelDate(DateTime.UtcNow.AddDays(-10))
            .WithEndDate(DateTime.UtcNow.AddDays(20))
            .Build();

        partner.PartnerLimits.Add(activeLimit);
        partner.PartnerLimits.Add(canceledLimit);

        return partner;
    }
}