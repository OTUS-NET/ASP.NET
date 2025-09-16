using System;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers.RoleMappers;

public class RoleMappers(IServiceProvider services) : IRoleMappers
{
    /// <inheritdoc />
    public IMapper<Role, RoleItemResponse> RoleToRoleItemResponse { get; }
        = services.GetRequiredService<IMapper<Role, RoleItemResponse>>();
}