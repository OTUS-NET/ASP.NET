using AutoMapper;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models.Request;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Mapping
{
    public class EmployeesMappingProfile : Profile
    {
        public EmployeesMappingProfile()
        {
            CreateMap<CreateOrEditEmployeeRequest, Employee>();
            CreateMap<Employee, EmployeeResponse>();
            CreateMap<Employee, EmployeeShortResponse>();
        }
    }
}
