using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Contracts.PromoCodes;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Queries.PromoCodes;

public class GetPromoCodesQuery : IRequest<IEnumerable<PromoCodeResponseDto>>
{
    public required GetPromoCodeRequestDto Data { get; set; }
}

public class GetPromoCodesQueryHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<GetPromoCodesQuery, IEnumerable<PromoCodeResponseDto>>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<IEnumerable<PromoCodeResponseDto>> Handle(GetPromoCodesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.PromoCodes.AsQueryable();

        if (request.Data.PreferenceId.HasValue)
        {
            query = query.Where(p => p.PreferenceId == request.Data.PreferenceId.Value);
        }

        if (DateTime.TryParse(request.Data.FromDate, out var parsedFromDate))
        {
            query = query.Where(p => p.BeginDate >= parsedFromDate);
        }

        if (DateTime.TryParse(request.Data.ToDate, out var parsedToDate))
        {
            query = query.Where(p => p.EndDate <= parsedToDate);
        }

        var promoCodes = await query
            .Select(p => new PromoCodeResponseDto
            {
                Id = p.Id,
                Code = p.Code,
                ServiceInfo = p.ServiceInfo,
                BeginDate = p.BeginDate,
                EndDate = p.EndDate,
                PartnerName = p.PartnerName,
                PreferenceId = p.PreferenceId
            })
            .ToListAsync(cancellationToken);

        return promoCodes;
    }
}