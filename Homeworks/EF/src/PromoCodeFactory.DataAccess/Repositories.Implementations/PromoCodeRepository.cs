using System;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.DataAccess.Repositories.Implementations;

public class PromoCodeRepository : EfRepository<PromoCode, Guid>, IPromoCodeRepository
{
    public PromoCodeRepository(DatabaseContext databaseContext) : base(databaseContext)
    {
    }
}