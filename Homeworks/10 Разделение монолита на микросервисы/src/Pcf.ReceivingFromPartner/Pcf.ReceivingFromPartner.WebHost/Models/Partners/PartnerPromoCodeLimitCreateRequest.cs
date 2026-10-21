using System.ComponentModel.DataAnnotations;
using Pcf.Common.Web.Validation;

namespace Pcf.ReceivingFromPartner.WebHost.Models.Partners;

public record PartnerPromoCodeLimitCreateRequest(
    [FutureDate(ErrorMessage = "EndAt must be greater than current time.")]
    DateTimeOffset EndAt,

    [Range(1, int.MaxValue, ErrorMessage = "Limit must be greater than zero.")]
    int Limit);
