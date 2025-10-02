using System;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class CustomerPromoCode : BaseEntity
{
    public Guid CustomersId { get; set; }
    public Guid PromoCodesId { get; set; }
}