using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Services.Partners.Abstractions;
using PromoCodeFactory.Services.Partners.Dto;
using PromoCodeFactory.Services.Partners.Exceptions;
using PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.Model;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetPartnerPromoCodeLimitAsyncTests
{
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnersServiceThrowsPartnerNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var model = CreateModel(new PartnerNotFoundException());

        // Act
        var result = await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.Request);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }
        
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnersServiceThrowsPartnerIsNotActiveException_ReturnsBadRequest()
    {
        // Arrange
        var model = CreateModel(new PartnerIsNotActiveException());

        // Act
        var result = await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.Request);

        // Assert
        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }
        
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnersServiceThrowsIncorrectLimitException_ReturnsBadRequest()
    {
        // Arrange
        var model = CreateModel(new IncorrectLimitException());

        // Act
        var result = await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.Request);

        // Assert
        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    private static PartnersControllerTestsModel CreateModel(Exception exception = null)
    {
        var model = new PartnersControllerTestsModel();
            
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
        if (exception != null)
        {
            fixture.Freeze<Mock<IPartnersService>>().Setup(s => s.SetPartnerPromoCodeLimitAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<SetPartnerPromoCodeLimitDto>()))
                .ThrowsAsync(exception);
        }

        model.Request = fixture.Create<SetPartnerPromoCodeLimitRequest>();
        model.Controller = fixture.Build<PartnersController>().OmitAutoProperties().Create();

        return model;
    }
}