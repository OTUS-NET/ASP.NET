using System;
using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mapping;

public class PromoCodeMappingsProfile : Profile
{
    public PromoCodeMappingsProfile()
    {
        CreateMap<GivePromoCodeRequest, PromoCode
            >()
            .ForMember(
                x => x.BeginDate,
                opt => opt.MapFrom(
                    src => DateTime.ParseExact(src.BeginDate, "dd/MM/yyyy", null)))
            .ForMember(
                x => x.EndDate,
                opt => opt.MapFrom(
                    src => DateTime.ParseExact(src.EndDate, "dd/MM/yyyy", null)));
        
        CreateMap<PromoCodeShortResponse, PromoCode>();
        CreateMap<PromoCode, PromoCodeShortResponse>();
    }
}