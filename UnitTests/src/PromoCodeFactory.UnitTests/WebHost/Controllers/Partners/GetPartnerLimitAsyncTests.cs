using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PromoCodeFactory.Core.Domain.Base;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.WebHost.Controllers;
using AutoFixture.Xunit2;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.UnitTests.Helps;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Helpers;

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
            result.Result.Should().BeAssignableTo<NotFoundResult>();
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
