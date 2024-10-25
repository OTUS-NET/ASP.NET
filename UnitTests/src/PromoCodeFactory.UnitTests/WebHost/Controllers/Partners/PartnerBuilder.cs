namespace DefaultNamespace;
/// <summary>
/// В качестве дополнительного условия тестовые данные должны формироваться с помощью Builder
/// </summary>
public static class PartnerBuilder
{
    public static Partner CreateActivePartner(Guid id)
    {
        return new Partner
        {
            Id = id,
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>()
        };
    }

    public static Partner CreateInactivePartner(Guid id)
    {
        return new Partner
        {
            Id = id,
            IsActive = false,
            PartnerLimits = new List<PartnerPromoCodeLimit>()
        };
    }

    public static Partner CreatePartnerWithActiveLimit(Guid id, int limit)
    {
        return new Partner
        {
            Id = id,
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>
            {
                new PartnerPromoCodeLimit { Limit = limit, CancelDate = null }
            }
        };
    }
}

public static class SetPartnerPromoCodeLimitRequestBuilder
{
    public static SetPartnerPromoCodeLimitRequest CreateValidRequest(int limit, DateTime endDate)
    {
        return new SetPartnerPromoCodeLimitRequest
        {
            Limit = limit,
            EndDate = endDate
        };
    }

    public static SetPartnerPromoCodeLimitRequest CreateInvalidLimitRequest(int limit)
    {
        return new SetPartnerPromoCodeLimitRequest
        {
            Limit = limit,
            EndDate = DateTime.Now
        };
    }
}