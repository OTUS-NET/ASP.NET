using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Request;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Mapping
{
    public class CustomerMappingProfile:Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<CreateOrEditCustomerRequest, Customer>();
            CreateMap<Customer, CustomerShortResponse>();
            CreateMap<Customer, CustomerResponse>()
                .ForMember(d => d.Preferences, map => map.MapFrom(m => m.CustomerPreferences));
        }
    }
}
