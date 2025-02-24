using AutoMapper;
using PromoCodeFactory.Services.Partners.Dto;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mapping;

public class PartnerMappingsProfile : Profile
{
    public PartnerMappingsProfile()
    {
        CreateMap<PartnerPromoCodeLimitDto, PartnerPromoCodeLimitResponse>();
        CreateMap<PartnerDto, PartnerResponse>();
        CreateMap<SetPartnerPromoCodeLimitRequest, SetPartnerPromoCodeLimitDto>();
    }
}