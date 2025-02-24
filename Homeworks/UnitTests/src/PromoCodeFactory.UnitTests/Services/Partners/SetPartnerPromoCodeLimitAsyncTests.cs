using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Partners.Exceptions;
using PromoCodeFactory.UnitTests.Services.Partners.Model.Builder;
using Xunit;

namespace PromoCodeFactory.UnitTests.Services.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ThrowsPartnerNotFoundException()
        {
            // Arrange
            var model = PartnersServiceTestsModelBuilder.Build().Create();

            // Act
            var action = async () => await model.Service.SetPartnerPromoCodeLimitAsync(
                Guid.NewGuid(),
                model.SetPartnerPromoCodeLimitDto);

            // Assert
            await action.Should().ThrowAsync<PartnerNotFoundException>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ThrowsPartnerIsNotActiveException()
        {
            // Arrange
            var model = PartnersServiceTestsModelBuilder.Build().WithPartner(isActive: false).Create();

            // Act
            var action = async () => await model.Service.SetPartnerPromoCodeLimitAsync(
                Guid.NewGuid(),
                model.SetPartnerPromoCodeLimitDto);

            // Assert
            await action.Should().ThrowAsync<PartnerIsNotActiveException>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_NumberIssuedPromoCodesShouldBeZero()
        {
            // Arrange
            var model = PartnersServiceTestsModelBuilder.Build().WithPartner(
                    withActiveLimit: true,
                    numberIssuedPromoCodes: 1).
                WithSetPartnerPromoCodeLimitDto().
                Create();

            // Act
            await model.Service.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.SetPartnerPromoCodeLimitDto);

            // Assert
            model.Partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_CancelDateShouldBeCurrentDate()
        {
            // Arrange
            var dateTime = new DateTime();
            var model = PartnersServiceTestsModelBuilder.Build().WithPartner(
                    withActiveLimit: true,
                    numberIssuedPromoCodes: 1).
                WithSetPartnerPromoCodeLimitDto().
                WithDateTimeProviderMock(dateTime).
                Create();

            // Act
            await model.Service.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.SetPartnerPromoCodeLimitDto);

            // Assert
            model.ActiveLimit.CancelDate.Should().Be(dateTime);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_RequestHasLimitLessOrEqualZero_ThrowIncorrectLimitException()
        {
            // Arrange
            var model = PartnersServiceTestsModelBuilder.Build().WithPartner().WithSetPartnerPromoCodeLimitDto(0).Create();
            
            // Act
            var action = async () => await model.Service.SetPartnerPromoCodeLimitAsync(
                Guid.NewGuid(), 
                model.SetPartnerPromoCodeLimitDto);

            // Assert
            await action.Should().ThrowAsync<IncorrectLimitException>();
        }
        
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_NewLimit_ShouldAddToDatabase()
        {
            // Arrange
            var model = PartnersServiceTestsModelBuilder.Build().WithPartner().WithSetPartnerPromoCodeLimitDto().Create();
            
            // Act
            await model.Service.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), model.SetPartnerPromoCodeLimitDto);

            model.PromoCodeLimitCollectionMock.Verify(
                c => c.Add(It.IsAny<PartnerPromoCodeLimit>()),
                Times.Once);
            model.PartnersRepositoryMock.Verify(r => r.UpdateAsync(model.Partner), Times.Once);
        }
    }
}