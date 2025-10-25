using System.ComponentModel.DataAnnotations;

namespace Pcf.GivingToCustomer.GraphQLHost.Dtos
{
    public class CustomerUpdateInput
    {
        public Guid Id { get; set; }

        [Required]
        public string? FirstName { get; set; }
        
        [Required]
        public string? LastName { get; set; }
        
        [Required]
        public string? Email { get; set; }

        public List<Guid>? PreferenceIds { get; set; }
    }
}
