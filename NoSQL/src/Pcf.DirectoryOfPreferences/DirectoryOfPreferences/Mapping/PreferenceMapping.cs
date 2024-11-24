using AutoMapper;
using DirectoryOfPreferences.Application.Models.Preference;
using DirectoryOfPreferences.Model.Request;
using DirectoryOfPreferences.Model.Response;

namespace DirectoryOfPreferences.Mapping
{
    public class PreferenceMapping: Profile
    {
        public PreferenceMapping()
        {
            CreateMap<PreferenceRequest, CreatePreferenceModel>();
            CreateMap<PreferenceModel, PreferenceResponse>();
        }
    }
}
