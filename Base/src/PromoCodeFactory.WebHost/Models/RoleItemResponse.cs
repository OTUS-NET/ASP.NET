using PromoCodeFactory.Core.Domain.Administration;
using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class RoleItemResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public RoleItemResponse(Role source)
        {
            Id = source.Id;
            Name = source.Name;
            Description = source.Description;
        }
    }
}