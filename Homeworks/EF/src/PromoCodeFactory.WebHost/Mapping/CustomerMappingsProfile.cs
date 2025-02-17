using AutoMapper;
using PromoCodeFactory.Services.Contracts.Customer;
using PromoCodeFactory.WebHost.Models.Customer;

namespace PromoCodeFactory.WebHost.Mapping;

public class CustomerMappingsProfile : Profile
{
    public CustomerMappingsProfile()
    {
        CreateMap<CustomerShortDto, CustomerShortResponse>();
        CreateMap<CustomerDto, CustomerResponse>()
            .ForMember(
                cr => cr.Preferences,
                map => map.MapFrom(cd => cd.Preferences))
            .ForMember(
                cr => cr.PromoCodes,
                map => map.MapFrom(cd => cd.PromoCodes));
        CreateMap<CreateOrEditCustomerDto, CreateOrEditCustomerRequest>();
        CreateMap<CreateOrEditCustomerRequest, CreateOrEditCustomerDto>();
    }
}