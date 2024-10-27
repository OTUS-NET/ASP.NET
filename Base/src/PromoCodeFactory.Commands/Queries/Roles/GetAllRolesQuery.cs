using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Contracts.Roles;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Queries.Roles;

public class GetAllRolesQuery : IRequest<List<RoleItemResponse>>
{
}

public class GetAllRolesQueryHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<GetAllRolesQuery, List<RoleItemResponse>>
{
    public async Task<List<RoleItemResponse>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await dbContext.Roles
            .AsNoTracking()
            .Select(x => new RoleItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);

        return roles;
    }
}