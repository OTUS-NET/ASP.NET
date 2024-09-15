using System.Collections.Generic;

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<string> NamesRoles { get; set; }
    public int AppliedPromocodesCount { get; set; }
}