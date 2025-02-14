using System.Collections.Generic;
using AutoMapper;
using PromoCodeFactory.Services.Contracts.Preference;
using PromoCodeFactory.WebHost.Models.Preference;

namespace PromoCodeFactory.WebHost.Mapping;

public class PreferenceMappingsProfile : Profile
{
    public PreferenceMappingsProfile()
    {
        CreateMap<PreferenceShortResponse, PreferenceShortDto>();
        CreateMap<PreferenceShortDto, PreferenceShortResponse>();
    }
}