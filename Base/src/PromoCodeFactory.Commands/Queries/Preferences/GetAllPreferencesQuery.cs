using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Contracts.Preferences;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Queries.Preferences;

public class GetAllPreferencesQuery : IRequest<List<PreferenceResponseDto>>;

public class GetAllPreferencesQueryHandler(PromoCodesDbContext dbContext) : IRequestHandler<GetAllPreferencesQuery, List<PreferenceResponseDto>>
{
    public Task<List<PreferenceResponseDto>> Handle(GetAllPreferencesQuery request, CancellationToken cancellationToken)
    {
        return dbContext.Preferences
            .Select(x => new PreferenceResponseDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }
}