using System.ComponentModel.DataAnnotations;

namespace Pcf.GivingToCustomer.GraphQLHost.Dtos
{
    public class PromoCodeInput
    {
        [Required]
        public string? Code { get; set; }

        [Required]
        public string? ServiceInfo { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid PreferenceId { get; set; }

        public List<Guid>? CustomerIds { get; set; }
    }
}
