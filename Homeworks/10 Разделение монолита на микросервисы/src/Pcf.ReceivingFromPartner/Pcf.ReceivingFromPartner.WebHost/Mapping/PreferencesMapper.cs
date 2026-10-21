using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Models.Preferences;

namespace Pcf.ReceivingFromPartner.WebHost.Mapping;

public static class PreferencesMapper
{
    public static PreferenceShortResponse ToPreferenceShortResponse(Preference preference)
    {
        return new PreferenceShortResponse(preference.Id, preference.Name);
    }
}
