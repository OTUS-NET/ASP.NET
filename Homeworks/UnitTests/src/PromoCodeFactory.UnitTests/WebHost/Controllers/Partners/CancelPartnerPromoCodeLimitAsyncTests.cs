using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Partners.Abstractions;
using PromoCodeFactory.Services.Partners.Exceptions;
using PromoCodeFactory.WebHost.Controllers;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class CancelPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IPartnersService> _partnersServiceMock;
        private readonly PartnersController _partnersController;

        public CancelPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersServiceMock = fixture.Freeze<Mock<IPartnersService>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }
        
        [Fact]
        public async Task CancelPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            
            _partnersServiceMock.Setup(service => service.CancelPartnerPromoCodeLimitAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new PartnerNotFoundException());

            // Act
            var result = await _partnersController.CancelPartnerPromoCodeLimitAsync(partnerId);
 
            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }
        
        [Fact]
        public async Task CancelPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            
            _partnersServiceMock.Setup(service => service.CancelPartnerPromoCodeLimitAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new PartnerIsNotActiveException());

            // Act
            var result = await _partnersController.CancelPartnerPromoCodeLimitAsync(partnerId);
 
            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }
    }
}