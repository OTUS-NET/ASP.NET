using Pcf.Common.Core.Domain;

namespace Pcf.Administration.Core.Domain;

public class Employee : BaseEntity
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public required string Email { get; set; }

    public required Role Role { get; set; }
}
