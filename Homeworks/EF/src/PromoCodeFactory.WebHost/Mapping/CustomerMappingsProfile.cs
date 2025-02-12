using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Contracts.Customer;
using PromoCodeFactory.WebHost.Models.Customer;

namespace PromoCodeFactory.WebHost.Mapping;

public class CustomerMappingsProfile : Profile
{
    public CustomerMappingsProfile()
    {
        CreateMap<CustomerShortDto, CustomerShortResponse>();
        CreateMap<CustomerDto, CustomerResponse>();
        CreateMap<CreateOrEditCustomerDto, CreateOrEditCustomerRequest>();
    }
}