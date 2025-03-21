using AutoMapper;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models.Responses;

namespace PromoCodeFactory.WebHost.Mapping
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<RoleItemResponse, Role>();
            CreateMap<Role, RoleItemResponse>();
        }
    }
}