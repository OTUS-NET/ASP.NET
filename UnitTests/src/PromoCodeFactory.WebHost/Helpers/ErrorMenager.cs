using Microsoft.AspNetCore.Http.HttpResults;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Helpers
{
    public static class ErrorMessages
    {
        public static string PartnerHasNotBeenFound() =>
            "The partner has not been found / Данный партнер не найден";
        public static string PartnerIsNotActive() =>
            "The partner is not active / Данный партнер не активен";
        public static string LimitMustBeGreaterThanZero() =>
            "Limit should be greater than zero / Лимит должен быть больше 0";
    }
}