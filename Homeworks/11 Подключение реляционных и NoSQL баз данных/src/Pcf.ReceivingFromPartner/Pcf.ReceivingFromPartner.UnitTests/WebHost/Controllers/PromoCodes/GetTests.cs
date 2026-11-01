using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Controllers;
using Pcf.ReceivingFromPartner.WebHost.Models.PromoCodes;

namespace Pcf.ReceivingFromPartner.UnitTests.WebHost.Controllers.PromoCodes;

public class GetTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly PromoCodesController _sut;

    public GetTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _sut = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            Mock.Of<IRepository<Partner>>(),
            Mock.Of<IRepository<Preference>>(),
            Mock.Of<IGivingToCustomerGateway>());
    }

    [Fact]
    public async Task Get_ReturnsPromoCodes()
    {
        var promoCode = CreatePromoCode();

        _promoCodesRepositoryMock
            .Setup(r => r.GetAll(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromoCode> { promoCode });

        var result = await _sut.Get(CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeAssignableTo<IEnumerable<PromoCodeShortResponse>>().Subject.Single();
        response.Id.Should().Be(promoCode.Id);
        response.Code.Should().Be(promoCode.Code);
        response.PartnerId.Should().Be(promoCode.Partner.Id);
        response.PreferenceId.Should().Be(promoCode.Preference.Id);
    }

    [Fact]
    public async Task GetById_WhenPromoCodeNotFound_ReturnsNotFound()
    {
        var promoCodeId = Guid.NewGuid();

        _promoCodesRepositoryMock
            .Setup(r => r.GetById(promoCodeId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoCode?)null);

        var result = await _sut.GetById(promoCodeId, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_WhenPromoCodeExists_ReturnsPromoCode()
    {
        var promoCode = CreatePromoCode();

        _promoCodesRepositoryMock
            .Setup(r => r.GetById(promoCode.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(promoCode);

        var result = await _sut.GetById(promoCode.Id, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<PromoCodeShortResponse>().Subject;
        response.Id.Should().Be(promoCode.Id);
        response.Code.Should().Be("THEATER");
        response.ServiceInfo.Should().Be("Tickets");
        response.PartnerId.Should().Be(promoCode.Partner.Id);
        response.PreferenceId.Should().Be(promoCode.Preference.Id);
    }

    private static PromoCode CreatePromoCode()
    {
        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Theater",
            IsActive = true,
            ManagerId = Guid.NewGuid()
        };
        var preference = new Preference
        {
            Id = Guid.NewGuid(),
            Name = "Theater"
        };

        return new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = "THEATER",
            ServiceInfo = "Tickets",
            BeginDate = DateTimeOffset.UtcNow,
            EndDate = DateTimeOffset.UtcNow.AddDays(10),
            Partner = partner,
            Preference = preference
        };
    }
}
