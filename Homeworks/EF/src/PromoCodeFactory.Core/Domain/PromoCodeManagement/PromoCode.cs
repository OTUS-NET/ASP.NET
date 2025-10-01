using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PromoCode
        : BaseEntity
    {
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(150)]
        public string ServiceInfo { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        [StringLength(150)]
        public string PartnerName { get; set; }

        public Employee PartnerManager { get; set; }

        public Preference Preference { get; set; }
        
        public ICollection<Customer> Customers { get; set; }
    }
}