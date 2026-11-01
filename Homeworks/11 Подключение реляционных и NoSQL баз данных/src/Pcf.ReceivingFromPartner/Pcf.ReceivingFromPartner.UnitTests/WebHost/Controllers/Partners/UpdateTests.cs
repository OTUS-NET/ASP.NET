using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Core.Exceptions;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Controllers;
using Pcf.ReceivingFromPartner.WebHost.Models.Partners;

namespace Pcf.ReceivingFromPartner.UnitTests.WebHost.Controllers.Partners;

public class UpdateTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IAdministrationGateway> _administrationGatewayMock;
    private readonly PartnersController _sut;

    public UpdateTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _administrationGatewayMock = new Mock<IAdministrationGateway>();
        _sut = new PartnersController(
            _partnersRepositoryMock.Object,
            Mock.Of<IRepository<PartnerPromoCodeLimit>>(),
            _administrationGatewayMock.Object);
    }

    [Fact]
    public async Task Update_WhenPartnerNotFound_ReturnsNotFound()
    {
        var partnerId = Guid.NewGuid();
        var request = CreateRequest();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        var result = await _sut.Update(partnerId, request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
        _administrationGatewayMock.Verify(
            g => g.IsPartnerManager(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenManagerIsNotPartnerManager_ReturnsBadRequest()
    {
        var partner = CreatePartner();
        var request = CreateRequest();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _administrationGatewayMock
            .Setup(g => g.IsPartnerManager(request.ManagerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Update(partner.Id, request, CancellationToken.None);

        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var problemDetails = badRequest.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Invalid manager");
        partner.Name.Should().Be("Theater");

        _partnersRepositoryMock.Verify(
            r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenValidRequest_ReturnsOkAndUpdatesPartner()
    {
        var partner = CreatePartner();
        var request = CreateRequest();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _administrationGatewayMock
            .Setup(g => g.IsPartnerManager(request.ManagerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Update(partner.Id, request, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<PartnerResponse>().Subject;
        response.Id.Should().Be(partner.Id);
        response.Name.Should().Be(request.Name);
        response.IsActive.Should().Be(request.IsActive);
        response.ManagerId.Should().Be(request.ManagerId);

        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        var partner = CreatePartner();
        var request = CreateRequest();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _administrationGatewayMock
            .Setup(g => g.IsPartnerManager(request.ManagerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partner.Id));

        var result = await _sut.Update(partner.Id, request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private static PartnerUpdateRequest CreateRequest()
    {
        return new PartnerUpdateRequest("Renamed theater", false, Guid.NewGuid());
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
