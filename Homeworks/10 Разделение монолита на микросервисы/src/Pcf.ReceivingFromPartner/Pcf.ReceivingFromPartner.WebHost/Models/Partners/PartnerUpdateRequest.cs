using System.ComponentModel.DataAnnotations;

namespace Pcf.ReceivingFromPartner.WebHost.Models.Partners;

public record PartnerUpdateRequest(
    [Required(ErrorMessage = "Name is required")]
    [StringLength(256, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 256 characters")]
    string Name,

    bool IsActive,

    [Required(ErrorMessage = "ManagerId is required")]
    Guid ManagerId);
