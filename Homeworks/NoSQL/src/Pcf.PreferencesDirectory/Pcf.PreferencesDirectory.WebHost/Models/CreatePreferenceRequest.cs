using System.ComponentModel.DataAnnotations;

namespace Pcf.PreferencesDirectory.WebHost.Models
{
    public class CreatePreferenceRequest
    {
        [Required, StringLength(200, MinimumLength = 3)]
        public required string Name { get; set; }
    }
}
