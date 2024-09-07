﻿using System;
using System.Collections.Generic;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.WebHost.Models
{
    public class PartnerResponse
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }
        
        public string Name { get; set; }

        public int NumberIssuedPromoCodes  { get; set; }

        public List<PartnerPromoCodeLimitResponse> PartnerLimits { get; set; }
    }
}