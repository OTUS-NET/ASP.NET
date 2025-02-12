using AutoMapper;
using PromoCodeFactory.Services.Contracts.Employee;
using PromoCodeFactory.WebHost.Models.Employee;

namespace PromoCodeFactory.WebHost.Mapping;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<EmployeeShortResponse, EmployeeShortDto>();
        CreateMap<EmployeeResponse, EmployeeDto>();
    }
}