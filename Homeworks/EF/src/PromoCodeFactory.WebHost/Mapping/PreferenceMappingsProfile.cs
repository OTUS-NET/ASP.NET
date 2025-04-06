using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mapping;

public class PreferenceMappingsProfile : Profile
{
    public PreferenceMappingsProfile()
    {
        CreateMap<PreferenceShortResponse, Preference>();
        CreateMap<Preference, PreferenceShortResponse>();
    }
}