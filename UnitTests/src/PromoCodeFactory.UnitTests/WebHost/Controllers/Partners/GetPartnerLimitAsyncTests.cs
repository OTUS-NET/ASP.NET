using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.UnitTests.Helps;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class GetPartnerLimitAsyncTests
    {
        [Theory, AutoMoqData]
        public async Task GetPartnerLimitAsync_PartnerIsNotFound_ReturnsNotFound(Guid partnerId, Guid limitId, [Frozen] Mock<IPartnerRepository> partnerRepositoryMock, PartnersController partnersController)
        {
            //Arrange
            Partner? partner = null;
            partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            //Act
            var result = await partnersController.GetPartnerLimitAsync(partnerId, limitId);
            //Assert
            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
            if (result.Result is NotFoundObjectResult notFoundResult)
                notFoundResult.Value.Should().BeEquivalentTo(ErrorMessages.PartnerHasNotBeenFound());

        }

        [Theory, AutoMoqData]
        public async Task GetPartnerLimitAsync_PartnerIsNotActive_ReturnsBadRequest(
            Partner partner,
            Guid limitId,
            [Frozen] Mock<IPartnerRepository> partnerRepositoryMock,
            PartnersController partnersController)
        {
            //Arrange
            partner.IsActive = false;
            partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            //Act
            var result = await partnersController.GetPartnerLimitAsync(partner.Id, limitId);
            //Assert
            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
            if (result.Result is BadRequestObjectResult badResult)
                badResult.Value.Should().BeEquivalentTo(ErrorMessages.PartnerHasNotBeenFound());
        }
    }
}