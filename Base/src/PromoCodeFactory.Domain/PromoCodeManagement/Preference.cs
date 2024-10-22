using System.Collections.Generic;

namespace PromoCodeFactory.Core.PromoCodeManagement;

public class Preference : BaseEntity
{
    public string Name { get; set; }
    
    public ICollection<CustomerPreference> CustomerPreferences { get; set; }
}