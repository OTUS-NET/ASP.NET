using System;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.DataAccess.Repositories.Implementations;

public class RoleRepository : EfRepository<Role, Guid>, IRoleRepository
{
    public RoleRepository(DatabaseContext databaseContext) : base(databaseContext)
    {
    }
}