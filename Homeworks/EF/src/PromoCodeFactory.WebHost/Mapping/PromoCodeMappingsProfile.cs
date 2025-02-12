using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using PromoCodeFactory.Services.Contracts.PromoCode;
using PromoCodeFactory.WebHost.Models.PromoCode;

namespace PromoCodeFactory.WebHost.Mapping;

public class PromoCodeMappingsProfile : Profile
{
    public PromoCodeMappingsProfile()
    {
        CreateMap<GivePromoCodeRequest, GivePromoCodeDto>()
            .ForMember(
                x => x.BeginDate,
                opt => opt.MapFrom(
                    src => DateTime.Parse(src.BeginDate, CultureInfo.InvariantCulture)))
            .ForMember(
                x => x.EndDate,
                opt => opt.MapFrom(
                    src => DateTime.Parse(src.EndDate, CultureInfo.InvariantCulture)));
        
        CreateMap<PromoCodeShortResponse, PromoCodeShortDto>();
        CreateMap<List<PromoCodeShortResponse>, List<PromoCodeShortDto>>();
        
        CreateMap<PromoCodeShortDto, PromoCodeShortResponse>();
        CreateMap<List<PromoCodeShortDto>, List<PromoCodeShortResponse>>();
    }
}