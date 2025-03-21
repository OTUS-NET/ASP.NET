using AutoMapper;
using PromoCodeFactory.WebHost.Models.Requests;
using PromoCodeFactory.WebHost.Models.Responses;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Mapping
{
    public class PromoCodeMappingProfile : Profile
    {
        public PromoCodeMappingProfile()
        {
            CreateMap<GivePromoCodeRequest, PromoCode>();
            CreateMap<PromoCode, PromoCodeShortResponse>();
        }
    }
}
