using System;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Domain
{
    public class Preference
        :BaseEntity
    {
        public string Name { get; set; }
    }
}