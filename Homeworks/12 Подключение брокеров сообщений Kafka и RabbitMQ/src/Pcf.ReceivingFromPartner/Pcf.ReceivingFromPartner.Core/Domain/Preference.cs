using Pcf.Common.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Domain;

public class Preference : BaseEntity
{
    public required string Name { get; set; }
}
