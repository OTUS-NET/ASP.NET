using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.WebHost.Models.Roles
{
    public static class RoleDtoExtensions
    {
        public static Role ToEntity(this RoleDto roleDto)
        {
            var result = new Role();
            result.Id = roleDto.Id;
            result.Name = roleDto.Name;
            result.Description = roleDto.Description;
            return result;
        }

        public static RoleDto ToDto(this Role entity)
        {
            var result = new RoleDto();
            result.Id = entity.Id;
            result.Name = entity.Name;
            result.Description = entity.Description;
            return result;
        }
    }
}
