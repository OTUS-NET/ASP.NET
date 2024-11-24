using AutoMapper;
using DirectoryOfPreferences.Application.Models.Preference;
using DirectoryOfPreferences.Domain.Entity;

namespace DirectoryOfPreferences.Application.Implementations.Mapping
{
    public class PreferenceMapping:Profile
    {
        public PreferenceMapping()
        {
            CreateMap<Preference, PreferenceModel>();
        }
    }
}
