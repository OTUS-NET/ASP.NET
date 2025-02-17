using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Contracts.Preference;

namespace PromoCodeFactory.Services.Implementations.Mapping;

public class PreferenceMappingsProfile : Profile
{
    public PreferenceMappingsProfile()
    {
        CreateMap<Preference, PreferenceShortDto>();
    }
}