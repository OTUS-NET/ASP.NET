using AutoFixture;
using FluentAssertions;
using Moq;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.WebHost.Controllers;
using Xunit;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.UnitTests.Helps;
using AutoFixture.Xunit2;
using PromoCodeFactory.WebHost.Helpers;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class CancelPartnerPromoCodeLimitAsyncTests
    {        
        [Theory, AutoMoqData]
        public async Task CancelPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound(
            Guid partnerId,
            [Frozen] Mock<IPartnerRepository> partnerRepositoryMock,
            PartnersController partnersController)
        {
            // Arrange
            Partner? partner = null;

            partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await partnersController.CancelPartnerPromoCodeLimitAsync(partnerId);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Theory, AutoMoqData]
        public async Task CancelPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest(
            Partner partner,
            [Frozen] Mock<IPartnerRepository> partnersRepositoryMock,
            PartnersController partnersController)
        {
            // Arrange
           var partnerId = partner.Id;
            partner.IsActive = false;

            partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await partnersController.CancelPartnerPromoCodeLimitAsync(partnerId);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            if (result is BadRequestObjectResult badResult)
                badResult.Value.Should().BeEquivalentTo(ErrorMessages.PartnerHasNotBeenFound());
        }
    }
}
