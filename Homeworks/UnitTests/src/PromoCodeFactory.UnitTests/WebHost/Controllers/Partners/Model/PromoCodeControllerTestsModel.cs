using System.Collections.Generic;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.Model;

public class PromoCodeControllerTestsModel
{
    public Partner Partner { get; set; }
    public SetPartnerPromoCodeLimitRequest Request { get; set; }
    public PartnersController Controller { get; set; }
    public PartnerPromoCodeLimit ActiveLimit { get; set; }
    public Mock<IRepository<Partner>> PartnersRepositoryMock { get; set; }
    public Mock<ICollection<PartnerPromoCodeLimit>> PromoCodeLimitCollectionMock { get; set; }
}