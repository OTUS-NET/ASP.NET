using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers;

public interface IRoleMappers
{
    IMapper<Role, RoleItemResponse> RoleToRoleItemResponse { get; }
    
}