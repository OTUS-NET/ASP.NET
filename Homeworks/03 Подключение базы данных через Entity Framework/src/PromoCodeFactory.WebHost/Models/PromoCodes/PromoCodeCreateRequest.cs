using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models.PromoCodes;

public record PromoCodeCreateRequest(
    [Required(ErrorMessage = "Code is required")]
    [StringLength(256, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    string Code,
    [Required(ErrorMessage = "Service info is required")]
    [StringLength(256, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    string ServiceInfo,
    [Required(ErrorMessage = "Partner name is required")]
    [StringLength(256, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    string PartnerName,
    [Required(ErrorMessage = "Begin date is required")]
    DateTimeOffset BeginDate,
    [Required(ErrorMessage = "End date is required")]
    DateTimeOffset EndDate,
    [Required(ErrorMessage = "Partner manager id is required")]
    Guid PartnerManagerId,
    [Required(ErrorMessage = "Preference id is required")]
    Guid PreferenceId);
