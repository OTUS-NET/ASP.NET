﻿using System;
using System.Collections.Generic;

namespace Pcf.ReceivingFromPartner.Core.Domain
{
    public class PromoCode
        : BaseEntity
    {
        public string Code { get; set; }

        public string ServiceInfo { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid PartnerId { get; set; }

        public virtual Partner Partner { get; set; }
        
        public Guid? PartnerManagerId { get; set; }
        
        public virtual Preference Preference { get; set; }

        public Guid PreferenceId { get; set; }
    }
}