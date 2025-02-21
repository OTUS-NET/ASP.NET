using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.Model.Builder;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var model = PromoCodeControllerTestsModelBuilder.Build().Create();

            // Act
            var result = await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), null);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var model = PromoCodeControllerTestsModelBuilder.Build().WithPartner(isActive: false).Create();

            // Act
            var result = await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), null);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_NumberIssuedPromoCodesShouldBeZero()
        {
            // Arrange
            var model = PromoCodeControllerTestsModelBuilder.Build().WithPartner(
                    withActiveLimit: true,
                    numberIssuedPromoCodes: 1).
                WithRequest().
                Create();

            // Act
            await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.Request);

            // Assert
            model.Partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_CancelDateShouldBeCurrentDate()
        {
            // Arrange
            var dateTime = new DateTime();
            var model = PromoCodeControllerTestsModelBuilder.Build().WithPartner(
                    withActiveLimit: true,
                    numberIssuedPromoCodes: 1).
                WithRequest().
                WithDateTimeProviderMock(dateTime).
                Create();

            // Act
            await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.Request);

            // Assert
            model.ActiveLimit.CancelDate.Should().Be(dateTime);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_RequestHasLimitLessOrEqualZero_ReturnsBadRequest()
        {
            // Arrange
            var model = PromoCodeControllerTestsModelBuilder.Build().WithPartner().WithRequest(0).Create();
            
            // Act
            var result = await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.Request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }
        
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_NewLimit_ShouldAddToDatabase()
        {
            // Arrange
            var model = PromoCodeControllerTestsModelBuilder.Build().WithPartner().WithRequest().Create();
            
            // Act
            await model.Controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.Request);

            model.PromoCodeLimitCollectionMock.Verify(
                c => c.Add(It.IsAny<PartnerPromoCodeLimit>()),
                Times.Once);
            model.PartnersRepositoryMock.Verify(r => r.UpdateAsync(model.Partner), Times.Once);
        }
    }
}