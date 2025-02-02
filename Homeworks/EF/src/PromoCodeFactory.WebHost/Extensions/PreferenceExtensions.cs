using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Extensions
{
    public static class PreferenceExtensions
    {
        /// <summary>
        /// Конвертирует Preference в PreferenceResponse
        /// </summary>
        public static PreferenceResponse ToResponse(this Preference preference)
        {
            return new PreferenceResponse
            {
                Id = preference.Id,
                Name = preference.Name
            };
        }
    }
}
