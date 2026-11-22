using Pcf.Common.Core.Domain;

namespace Pcf.Administration.Core.Domain;

public class Role : BaseEntity
{
    public required string Name { get; set; }

    public string? Description { get; set; }
}
