using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Partners.Dto;

namespace PromoCodeFactory.Services.Partners.Mapping;

public class PartnerMappingsProfile : Profile
{
    public PartnerMappingsProfile()
    {
        CreateMap<Partner, PartnerDto>()
            .ForMember(
                pd => pd.IsActive, 
                map => map.MapFrom(pd => true));
        
        CreateMap<PartnerPromoCodeLimit, PartnerPromoCodeLimitDto>()
            .ForMember(
                ld => ld.CreateDate,
                map => map.MapFrom(l => l.CreateDate.ToString("dd.MM.yyyy hh:mm:ss")))
            .ForMember(
                ld => ld.EndDate,
                map => map.MapFrom(l => l.EndDate.ToString("dd.MM.yyyy hh:mm:ss")))
            .ForMember(
                ld => ld.CancelDate,
                map => map.MapFrom(l => l.CancelDate != null ? l.CancelDate.Value.ToString("dd.MM.yyyy hh:mm:ss") : "" ));
    }
}