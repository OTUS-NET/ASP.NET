using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Services;
using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Services;

public class RoleService(IRepository<Role> _repository) : IRoleService
{
    public async Task<Role> GetById(Guid id, CancellationToken ct)
    {
        return await _repository.GetById(id, ct);
    }
}
