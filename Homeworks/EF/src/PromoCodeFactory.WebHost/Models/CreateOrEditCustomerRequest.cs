using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PromoCodeFactory.WebHost.Models
{
    public class CreateOrEditCustomerRequest
    {
        [JsonInclude]
        public string FirstName { get; set; } = string.Empty;
        
        [JsonInclude]
        public string LastName { get; set; } = string.Empty;
        
        [JsonInclude]
        public string Email { get; set; } = string.Empty;
        
        [JsonInclude]
        public List<Guid> PreferenceIds { get; set; } = [];
    }
}