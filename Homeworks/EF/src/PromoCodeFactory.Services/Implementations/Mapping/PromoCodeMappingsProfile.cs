using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Contracts.PromoCode;

namespace PromoCodeFactory.Services.Implementations.Mapping;

public class PromoCodeMappingsProfile : Profile
{
    public PromoCodeMappingsProfile()
    {
        CreateMap<PromoCode, PromoCodeShortDto>()
            .ForMember(
                x => x.BeginDate,
                opt => opt.MapFrom(
                    src => src.BeginDate.ToShortDateString()))
            .ForMember(
                x => x.EndDate,
                opt => opt.MapFrom(
                    src => src.EndDate.ToShortDateString()));

        CreateMap<GivePromoCodeDto, PromoCode>()
            .ForMember(
                x => x.Id, opt => opt.Ignore())
            .ForMember(
                x => x.Code, opt => opt.MapFrom(
                    src => src.PromoCode))
            .ForMember(
                x => x.PartnerManagerId, opt => opt.Ignore())
            .ForMember(
                x => x.PartnerManager, opt => opt.Ignore())
            .ForMember(
                x => x.PreferenceId, opt => opt.Ignore())
            .ForMember(
                x => x.Preference, opt => opt.Ignore())
            .ForMember(
                x => x.CustomerId, opt => opt.Ignore())
            .ForMember(
                x => x.Customer, opt => opt.Ignore());
    }
}