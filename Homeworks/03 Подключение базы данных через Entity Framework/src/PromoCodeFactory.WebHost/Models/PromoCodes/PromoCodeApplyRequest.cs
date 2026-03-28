using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models.PromoCodes;

public record PromoCodeApplyRequest([Required(ErrorMessage = "Customer id is required")] Guid CustomerId);
