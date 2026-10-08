using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Abstractions.Services;

public interface IRoleService
{
    Task<Role> GetById(Guid id, CancellationToken ct);
}
