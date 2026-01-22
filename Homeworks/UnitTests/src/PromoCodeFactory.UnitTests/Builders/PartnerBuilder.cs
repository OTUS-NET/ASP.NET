using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;

namespace PromoCodeFactory.UnitTests.Builders;

public class PartnerBuilder
{
    private readonly Partner _partner = new()
    {
        Id = Guid.NewGuid(),
        Name = "Test Partner",
        IsActive = true,
        NumberIssuedPromoCodes = 10,
        PartnerLimits = []
    };

    public PartnerBuilder WithId(Guid id)
    {
        _partner.Id = id;
        return this;
    }

    public PartnerBuilder WithName(string name)
    {
        _partner.Name = name;
        return this;
    }

    public PartnerBuilder SetActive(bool isActive)
    {
        _partner.IsActive = isActive;
        return this;
    }

    public PartnerBuilder WithIssuedPromoCodes(int count)
    {
        _partner.NumberIssuedPromoCodes = count;
        return this;
    }

    public PartnerBuilder WithPartnerLimit(PartnerPromoCodeLimit limit)
    {
        _partner.PartnerLimits.Add(limit);
        return this;
    }

    public PartnerBuilder WithActiveLimit(int limit = 100, DateTime? endDate = null)
    {
        var partnerLimit = new PartnerPromoCodeLimitBuilder()
            .WithPartnerId(_partner.Id)
            .WithLimit(limit)
            .WithEndDate(endDate ?? DateTime.UtcNow.AddDays(30))
            .Build();

        _partner.PartnerLimits.Add(partnerLimit);
        return this;
    }

    public PartnerBuilder WithExpiredLimit(int limit = 100)
    {
        var partnerLimit = new PartnerPromoCodeLimitBuilder()
            .WithPartnerId(_partner.Id)
            .WithLimit(limit)
            .WithEndDate(DateTime.UtcNow.AddDays(-1))
            .Build();

        _partner.PartnerLimits.Add(partnerLimit);
        return this;
    }

    public PartnerBuilder WithCanceledLimit(int limit = 100)
    {
        var partnerLimit = new PartnerPromoCodeLimitBuilder()
            .WithPartnerId(_partner.Id)
            .WithLimit(limit)
            .WithCancelDate(DateTime.UtcNow.AddDays(-1))
            .Build();

        _partner.PartnerLimits.Add(partnerLimit);
        return this;
    }

    public Partner Build()
    {
        return _partner;
    }
}