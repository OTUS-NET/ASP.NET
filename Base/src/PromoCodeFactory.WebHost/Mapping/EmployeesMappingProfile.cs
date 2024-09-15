using AutoMapper;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mapping
{
    public class EmployeesMappingProfile:Profile
    {
        public EmployeesMappingProfile()
        {
            CreateMap<CreateEmployeeRequest, Employee>();
            CreateMap<UpdateEmployeeRequest,Employee>();
            CreateMap<Employee,EmployeeResponse>();
            CreateMap<Employee,EmployeeShortResponse>();
        }
    }
}
