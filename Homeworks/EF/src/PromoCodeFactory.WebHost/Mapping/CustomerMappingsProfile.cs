using System.Collections.Generic;
using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mapping;

public class CustomerMappingsProfile : Profile
{
    public CustomerMappingsProfile()
    {
        CreateMap<Customer, CustomerShortResponse>();
        CreateMap<Customer, CustomerResponse>()
            .ForMember(
                cr => cr.Preferences,
                map => map.MapFrom(cd => cd.Preferences))
            .ForMember(
                cr => cr.PromoCodes,
                map => map.MapFrom(cd => cd.PromoCodes));
        CreateMap<Customer, CreateOrEditCustomerRequest>();
        CreateMap<CreateOrEditCustomerRequest, Customer>()
            .ForMember(c => c.Id, map => map.Ignore())
            .ForMember(c => c.PromoCodes, map => map.MapFrom(
                v => new List<PromoCode>()))
            .ForMember(c => c.Preferences, map => map.MapFrom(
                v => new List<Preference>()));
    }
}