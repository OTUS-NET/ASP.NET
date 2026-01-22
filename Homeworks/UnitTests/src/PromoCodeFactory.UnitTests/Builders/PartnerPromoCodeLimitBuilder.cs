using System;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.UnitTests.Builders;

public class PartnerPromoCodeLimitBuilder
{
    private readonly PartnerPromoCodeLimit _limit = new()
    {
        Id = Guid.NewGuid(),
        PartnerId = Guid.NewGuid(),
        Limit = 100,
        CreateDate = DateTime.UtcNow,
        EndDate = DateTime.UtcNow.AddDays(30),
        CancelDate = null
    };

    public PartnerPromoCodeLimitBuilder WithId(Guid id)
    {
        _limit.Id = id;
        return this;
    }

    public PartnerPromoCodeLimitBuilder WithPartnerId(Guid partnerId)
    {
        _limit.PartnerId = partnerId;
        return this;
    }

    public PartnerPromoCodeLimitBuilder WithLimit(int limit)
    {
        _limit.Limit = limit;
        return this;
    }

    public PartnerPromoCodeLimitBuilder WithCreateDate(DateTime createDate)
    {
        _limit.CreateDate = createDate;
        return this;
    }

    public PartnerPromoCodeLimitBuilder WithEndDate(DateTime endDate)
    {
        _limit.EndDate = endDate;
        return this;
    }

    public PartnerPromoCodeLimitBuilder WithCancelDate(DateTime? cancelDate)
    {
        _limit.CancelDate = cancelDate;
        return this;
    }

    public PartnerPromoCodeLimit Build()
    {
        return _limit;
    }
}