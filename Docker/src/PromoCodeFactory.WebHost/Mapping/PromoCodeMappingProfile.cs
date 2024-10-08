using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Request;
using PromoCodeFactory.WebHost.Models.Response;

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
