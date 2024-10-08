using Microsoft.AspNetCore.Http.HttpResults;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Helpers
{
    public static class ErrorMessages
    {
        public static string PartnerHasNotBeenFound() =>
            "Данный партнер не активен";
        public static string PartnerIsNotActive() =>
            "Данный партнер не активен";
        public static string LimitMustBeGreaterThanZero() =>
            "Лимит должен быть больше 0";
    }
}
