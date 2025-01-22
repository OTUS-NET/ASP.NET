using System;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.WebHost.Services;

public interface IRolesService
{
    Task<Role> GetByIdAsync(Guid id);
}