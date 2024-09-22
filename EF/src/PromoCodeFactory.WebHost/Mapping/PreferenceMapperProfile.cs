using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Mapping
{
    public class PreferenceMapperProfile:Profile
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
