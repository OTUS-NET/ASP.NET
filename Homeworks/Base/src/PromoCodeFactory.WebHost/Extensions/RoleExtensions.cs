using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.Core.Extensions
{
    public static class RoleExtensions
    {
        public static RoleItemResponse ToResponse(this Role entity)
        {
            return new RoleItemResponse()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };
        }
    }
}
