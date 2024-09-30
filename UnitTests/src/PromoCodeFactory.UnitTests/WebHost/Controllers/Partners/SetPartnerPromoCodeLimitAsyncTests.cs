using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetPartnerPromoCodeLimitAsyncTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly PartnersController _partnersController;

    public SetPartnerPromoCodeLimitAsyncTests()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
        _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        Partner partner = null;

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                               .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request: null);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
    {
        // Arrange
        var partner = new PartnerBuilder().WithActiveState(false)
                                          .Build();

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request: null);

        // Assert
        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SetPartnerPromoCodeLimitAsync_WithAndWithoutActiveLimit_IssuedNumberWasReset(bool withActiveLimit)
    {
        // Arrange
        var originIssuedNumber = 42;
        var partnerBuilder = new PartnerBuilder().WithActiveState(true)
                                                 .WithNumberIssuedPromoCodes(originIssuedNumber);

        if (withActiveLimit)
            partnerBuilder.WithActiveLimit();

        var partner = partnerBuilder.Build();
        var request = new SetPartnerPromoCodeLimitRequest()
        {
            Limit = 50
        };
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                               .ReturnsAsync(partner);

        // Act
        await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

        // Assert
        if (withActiveLimit)
            partner.NumberIssuedPromoCodes.Should().Be(0);
        else
            partner.NumberIssuedPromoCodes.Should().Be(originIssuedNumber);
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_WithActiveLimit_ActiveLimitCancelled()
    {
        // Arrange
        var partner = new PartnerBuilder().WithActiveState(true)
                                          .WithActiveLimit()
                                          .Build();
        var request = new SetPartnerPromoCodeLimitRequest()
        {
            Limit = 50
        };
        var activeLimit = partner.PartnerLimits.FirstOrDefault(x => !x.CancelDate.HasValue);
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                               .ReturnsAsync(partner);

        // Act
        await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

        // Assert
        activeLimit.CancelDate.Should().NotBeNull();
    }

    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_WithZeroRequestedLimit_ReturnsBadRequest()
    {
        // Arrange
        var partner = new PartnerBuilder().WithActiveState(true).Build();
        var request = new SetPartnerPromoCodeLimitRequest()
        {
            Limit = 0
        };
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                               .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

        // Assert
        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }
}