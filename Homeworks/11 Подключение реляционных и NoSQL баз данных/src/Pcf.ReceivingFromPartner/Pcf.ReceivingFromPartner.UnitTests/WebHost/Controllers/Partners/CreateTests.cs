using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Controllers;
using Pcf.ReceivingFromPartner.WebHost.Models.Partners;

namespace Pcf.ReceivingFromPartner.UnitTests.WebHost.Controllers.Partners;

public class CreateTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IAdministrationGateway> _administrationGatewayMock;
    private readonly PartnersController _sut;

    public CreateTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _administrationGatewayMock = new Mock<IAdministrationGateway>();
        _sut = new PartnersController(
            _partnersRepositoryMock.Object,
            Mock.Of<IRepository<PartnerPromoCodeLimit>>(),
            _administrationGatewayMock.Object);
    }

    [Fact]
    public async Task Create_WhenManagerIsNotPartnerManager_ReturnsBadRequest()
    {
        var request = CreateRequest();

        _administrationGatewayMock
            .Setup(g => g.IsPartnerManager(request.ManagerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Create(request, CancellationToken.None);

        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var problemDetails = badRequest.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Invalid manager");

        _partnersRepositoryMock.Verify(
            r => r.Add(It.IsAny<Partner>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenManagerIsPartnerManager_ReturnsCreatedAndAddsPartner()
    {
        var request = CreateRequest();

        _administrationGatewayMock
            .Setup(g => g.IsPartnerManager(request.ManagerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _partnersRepositoryMock
            .Setup(r => r.Add(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Create(request, CancellationToken.None);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(PartnersController.GetById));

        var response = created.Value.Should().BeOfType<PartnerResponse>().Subject;
        response.Name.Should().Be(request.Name);
        response.IsActive.Should().Be(request.IsActive);
        response.ManagerId.Should().Be(request.ManagerId);
        created.RouteValues!["partnerId"].Should().Be(response.Id);

        _partnersRepositoryMock.Verify(
            r => r.Add(It.Is<Partner>(p =>
                p.Id == response.Id &&
                p.Name == request.Name &&
                p.IsActive == request.IsActive &&
                p.ManagerId == request.ManagerId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static PartnerCreateRequest CreateRequest()
    {
        return new PartnerCreateRequest("Theater", true, Guid.NewGuid());
    }
}
