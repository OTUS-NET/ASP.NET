using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.Helps;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Helpers;
using PromoCodeFactory.WebHost.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        [Theory, AutoMoqData]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound(Guid partnerId, 
            SetPartnerPromoCodeLimitRequest limit, 
            [Frozen] Mock<IPartnerRepository> partnerRepositoryMock, 
            PartnersController partnersController)
        {
            //Arrange
            Partner? partner = null;
            partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            //Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, limit);
            //Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Theory, AutoMoqData]
        public async Task SetPartnerCodeLimitAsync_PartnerIsNotActive_ReturnBadRequest(Partner partner, 
            SetPartnerPromoCodeLimitRequest limit,
            [Frozen] Mock<IPartnerRepository> partnerRepositoryMock,
            PartnersController partnersController) 
        {
            //Arrange
            var partnerId = partner.Id;
            partner.IsActive = false;
            partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            //Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, limit);
            //Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            if (result is BadRequestObjectResult badResult) 
                badResult.Value.Should().BeEquivalentTo(ErrorMessages.PartnerHasNotBeenFound());
        }

        [Theory, AutoMoqData]
        public async Task SetPartnerCodeLimitAsync_SettingNewLimits_ResetTheLimitCounter(
            Partner partner,
            SetPartnerPromoCodeLimitRequest limit,
            [Frozen] Mock<IPartnerRepository> partnerRepositoryMock,
            PartnersController partnersController)
        {
            //Arrange
            partner.PartnerLimits.Last().CancelDate = null;
            var partnerId = partner.Id;
            partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            //Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, limit);
            //Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Theory, AutoMoqData]
        public async Task SetPartnerCodeLimitAsync_SettingNewLimits_DisablePreviousLimits(
            Partner partner,
            SetPartnerPromoCodeLimitRequest limit,
            [Frozen] Mock<IPartnerRepository> partnerRepositoryMock,
            PartnersController partnersController) 
        {
            //Arrange
            partner.PartnerLimits.Last().CancelDate = null;
            var partnerId = partner.Id;
            partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            //Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, limit);
            //Assert
            var count = partner.PartnerLimits.Count();
            partner.PartnerLimits[count-2].CancelDate.Should().NotBeNull();
        }

        [Theory, AutoMoqData]
        public async Task SetPartnerCodeLimitAsync_SettingNewLimits_LimitGreaterZero(
            Partner partner,
            SetPartnerPromoCodeLimitRequest limit,
            [Frozen] Mock<IPartnerRepository> partnerRepositoryMock,
            PartnersController partnersController)
        {
            //Arrange
            limit.Limit = -1 * limit.Limit;
            partner.PartnerLimits.Last().CancelDate = null;
            var partnerId = partner.Id;
            partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            //Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, limit);
            //Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            if (result is BadRequestObjectResult badResult)
                badResult.Value.Should().BeEquivalentTo(ErrorMessages.LimitMustBeGreaterThanZero());
        }
    }
}
