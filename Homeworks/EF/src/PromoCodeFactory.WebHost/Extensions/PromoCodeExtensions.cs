using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;

namespace PromoCodeFactory.WebHost.Extensions
{
    public static class PromoCodeExtensions
    {
        /// <summary>
        /// Конвертирует PromoCode в PromoCodeShortResponse
        /// </summary>
        public static PromoCodeShortResponse ToShortResponse(this PromoCode promoCode)
        {
            return new PromoCodeShortResponse
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
                PartnerName = promoCode.PartnerName,
            };
        }
    }
}
