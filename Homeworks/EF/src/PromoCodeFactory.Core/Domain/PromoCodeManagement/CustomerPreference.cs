using System;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class CustomerPreference : BaseEntity
{
    public Guid CustomersId { get; set; }
    public Guid PreferencesId { get; set; }
}