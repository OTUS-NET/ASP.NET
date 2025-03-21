using AutoMapper;
using PromoCodeFactory.WebHost.Models.Responses;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Mapping
{
    public class PreferenceMapperProfile : Profile
    {
        public PreferenceMapperProfile()
        {
            CreateMap<Preference, PreferenceResponse>();
            CreateMap<CustomerPreference, PreferenceResponse>()
                .ForMember(d => d.Name, map => map.MapFrom(m => m.Preference.Name))
                .ForMember(d => d.Description, map => map.MapFrom(m => m.Preference.Description));
        }
    }
}