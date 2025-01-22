using System;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.WebHost.Services;

public class RolesService : IRolesService
{
    private readonly IRepository<Role> _repository;

    public RolesService(IRepository<Role> repository)
    {
        _repository = repository;
    }

    public async Task<Role> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);
}