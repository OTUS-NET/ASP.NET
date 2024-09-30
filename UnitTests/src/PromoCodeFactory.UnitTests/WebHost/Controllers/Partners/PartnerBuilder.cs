using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public sealed class PartnerBuilder
{
    private readonly Partner partner;

    public PartnerBuilder()
    {
        partner = new()
        {
            Id = Guid.NewGuid(),
            Name = "Суперигрушки",
            PartnerLimits = []
        };
    }

    public PartnerBuilder WithId(Guid guid)
    {
        partner.Id = guid;

        return this;
    }

    public PartnerBuilder WithActiveState(bool isActive)
    {
        partner.IsActive = isActive;

        return this;
    }
    public PartnerBuilder WithNumberIssuedPromoCodes(int numberIssuedPromoCodes)
    {
        partner.NumberIssuedPromoCodes = numberIssuedPromoCodes;

        return this;
    }

    public PartnerBuilder WithActiveLimit()
    {
        partner.PartnerLimits.Add(
        new()
        {
            Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
            CreateDate = new DateTime(2020, 07, 9),
            EndDate = new DateTime(2020, 10, 9),
            Limit = 100
        });

        return this;
    }

    public Partner Build()
        => partner;
}
