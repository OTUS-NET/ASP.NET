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
                    src => DateTime.ParseExact(src.BeginDate, "dd/MM/yyyy", null)))
            .ForMember(
                x => x.EndDate,
                opt => opt.MapFrom(
                    src => DateTime.ParseExact(src.EndDate, "dd/MM/yyyy", null)));
        
        CreateMap<PromoCodeShortResponse, PromoCodeShortDto>();
        CreateMap<PromoCodeShortDto, PromoCodeShortResponse>();
    }
}