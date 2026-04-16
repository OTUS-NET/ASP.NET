using System.ComponentModel.DataAnnotations.Schema;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Entities;

public class PromoCode: BaseEntity
{
    public required string Code { get; set; }
    public required string ServiceInfo { get; set; }
    public required string PartnerName { get; set; }
    public DateTimeOffset BeginDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public Guid PartnerManagerId { get; set; }
    public Guid PreferenceId { get; set; }

    public Employee? PartnerManager { get; set; }
    public Preference? Preference { get; set; }
}
