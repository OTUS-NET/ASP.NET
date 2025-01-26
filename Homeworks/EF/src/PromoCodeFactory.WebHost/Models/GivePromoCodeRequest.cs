using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; }

        //Изменил эти поля на гуиды, чтобы не осуществлять поиск по имени в БД
        public Guid PartnerId { get; set; }

        public string PromoCode { get; set; }

        public Guid PreferenceId { get; set; }
    }
}