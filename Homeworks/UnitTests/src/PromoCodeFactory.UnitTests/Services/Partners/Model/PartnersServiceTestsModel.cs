using System.Collections.Generic;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Partners;
using PromoCodeFactory.Services.Partners.Dto;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.UnitTests.Services.Partners.Model;

public class PartnersServiceTestsModel
{
    public Partner Partner { get; set; }
    public SetPartnerPromoCodeLimitDto SetPartnerPromoCodeLimitDto { get; set; }
    public PartnersService Service { get; set; }
    public PartnerPromoCodeLimit ActiveLimit { get; set; }
    public Mock<IRepository<Partner>> PartnersRepositoryMock { get; set; }
    public Mock<ICollection<PartnerPromoCodeLimit>> PromoCodeLimitCollectionMock { get; set; }
}