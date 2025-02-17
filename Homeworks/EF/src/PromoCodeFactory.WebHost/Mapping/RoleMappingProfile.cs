using AutoMapper;
using PromoCodeFactory.Services.Contracts.Role;
using PromoCodeFactory.WebHost.Models.Role;

namespace PromoCodeFactory.WebHost.Mapping;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<RoleItemResponse, RoleItemDto>();
    }
}