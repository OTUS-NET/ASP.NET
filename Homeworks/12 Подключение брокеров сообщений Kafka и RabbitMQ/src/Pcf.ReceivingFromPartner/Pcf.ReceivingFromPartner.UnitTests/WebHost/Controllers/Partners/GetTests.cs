using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Controllers;
using Pcf.ReceivingFromPartner.WebHost.Models.Partners;

namespace Pcf.ReceivingFromPartner.UnitTests.WebHost.Controllers.Partners;

public class GetTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly PartnersController _sut;

    public GetTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _sut = new PartnersController(
            _partnersRepositoryMock.Object,
            Mock.Of<IRepository<PartnerPromoCodeLimit>>(),
            Mock.Of<IAdministrationGateway>());
    }

    [Fact]
    public async Task Get_ReturnsPartners()
    {
        var first = CreatePartner("Theater");
        var second = CreatePartner("School");

        _partnersRepositoryMock
            .Setup(r => r.GetAll(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Partner> { first, second });

        var result = await _sut.Get(CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var partners = ok.Value.Should().BeAssignableTo<IEnumerable<PartnerResponse>>().Subject.ToList();
        partners.Select(p => p.Id).Should().Equal(first.Id, second.Id);
        partners[0].Name.Should().Be("Theater");
        partners[1].ManagerId.Should().Be(second.ManagerId);
    }

    [Fact]
    public async Task GetById_WhenPartnerNotFound_ReturnsNotFound()
    {
        var partnerId = Guid.NewGuid();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        var result = await _sut.GetById(partnerId, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_WhenPartnerExists_ReturnsPartnerWithLimits()
    {
        var partner = CreatePartner("Theater");
        var older = CreateLimit(partner, DateTimeOffset.UtcNow.AddDays(-2));
        var newer = CreateLimit(partner, DateTimeOffset.UtcNow.AddDays(-1));
        partner.PartnerLimits = [older, newer];

        _partnersRepositoryMock
            .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        var result = await _sut.GetById(partner.Id, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<PartnerResponse>().Subject;
        response.Id.Should().Be(partner.Id);
        response.Name.Should().Be(partner.Name);
        response.IsActive.Should().BeTrue();
        response.ManagerId.Should().Be(partner.ManagerId);
        response.PartnerLimits.Select(l => l.Id).Should().Equal(newer.Id, older.Id);
    }

    private static Partner CreatePartner(string name)
    {
        return new Partner
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsActive = true,
            ManagerId = Guid.NewGuid()
        };
    }

    private static PartnerPromoCodeLimit CreateLimit(Partner partner, DateTimeOffset createdAt)
    {
        return new PartnerPromoCodeLimit
        {
            Id = Guid.NewGuid(),
            Partner = partner,
            CreatedAt = createdAt,
            EndAt = createdAt.AddDays(30),
            Limit = 10
        };
    }
}
