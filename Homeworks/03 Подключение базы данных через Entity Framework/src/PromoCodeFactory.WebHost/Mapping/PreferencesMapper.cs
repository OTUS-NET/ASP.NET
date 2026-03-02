using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Preferences;

namespace PromoCodeFactory.WebHost.Mapping;

public static class PreferencesMapper
{
    public static PreferenceShortResponse ToPreferenceShortResponse(Preference preference)
    {
        return new PreferenceShortResponse(preference.Id, preference.Name);
    }
}
