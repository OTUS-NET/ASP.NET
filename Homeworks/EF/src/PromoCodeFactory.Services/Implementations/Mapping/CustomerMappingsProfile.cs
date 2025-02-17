using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Contracts.Customer;

namespace PromoCodeFactory.Services.Implementations.Mapping;

public class CustomerMappingsProfile : Profile
{
    public CustomerMappingsProfile()
    {
        CreateMap<Customer, CustomerShortDto>();
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateOrEditCustomerDto, Customer>()
            .ForMember(c => c.Id, map => map.Ignore())
            .ForMember(c => c.PromoCodes, map => map.MapFrom(
                v => new List<PromoCode>()))
            .ForMember(c => c.Preferences, map => map.MapFrom(
                v => new List<Preference>()));
    }
}