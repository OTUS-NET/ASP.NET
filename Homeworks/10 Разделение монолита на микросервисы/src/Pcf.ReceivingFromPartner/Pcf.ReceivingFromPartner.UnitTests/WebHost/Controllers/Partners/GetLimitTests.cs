using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Controllers;
using Pcf.ReceivingFromPartner.WebHost.Models.Partners;

namespace Pcf.ReceivingFromPartner.UnitTests.WebHost.Controllers.Partners;

public class GetLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly PartnersController _sut;

    public GetLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _sut = new PartnersController(
            _partnersRepositoryMock.Object,
            Mock.Of<IRepository<PartnerPromoCodeLimit>>(),
            Mock.Of<IAdministrationGateway>());
    }

    [Fact]
    public async Task GetLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        var partnerId = Guid.NewGuid();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        var result = await _sut.GetLimit(partnerId, Guid.NewGuid(), CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetLimit_WhenLimitNotFound_ReturnsNotFound()
    {
        var partner = CreatePartner();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        var result = await _sut.GetLimit(partner.Id, Guid.NewGuid(), CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetLimit_WhenLimitExists_ReturnsLimit()
    {
        var partner = CreatePartner();
        var limit = new PartnerPromoCodeLimit
        {
            Id = Guid.NewGuid(),
            Partner = partner,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            EndAt = DateTimeOffset.UtcNow.AddDays(30),
            Limit = 50,
            IssuedCount = 3
        };
        partner.PartnerLimits = [limit];

        _partnersRepositoryMock
            .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        var result = await _sut.GetLimit(partner.Id, limit.Id, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>().Subject;
        response.Id.Should().Be(limit.Id);
        response.Limit.Should().Be(50);
        response.IssuedCount.Should().Be(3);
        response.CanceledAt.Should().BeNull();
    }

    private static Partner CreatePartner()
    {
        return new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Theater",
            IsActive = true,
            ManagerId = Guid.NewGuid()
        };
    }
}
