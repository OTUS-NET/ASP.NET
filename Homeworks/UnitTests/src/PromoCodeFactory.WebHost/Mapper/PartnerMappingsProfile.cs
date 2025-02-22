using AutoMapper;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Partners.Dto;

namespace PromoCodeFactory.WebHost.Mapper;

public class PartnerMappingsProfile : Profile
{
    public PartnerMappingsProfile()
    {
        CreateMap<PartnerDto, PartnerResponse>();
        CreateMap<SetPartnerPromoCodeLimitRequest, SetPartnerPromoCodeLimitDto>();
    }
}