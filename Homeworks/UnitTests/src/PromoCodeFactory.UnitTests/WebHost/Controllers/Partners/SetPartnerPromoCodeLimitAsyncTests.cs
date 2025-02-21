using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Partner)null);

            var partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();

            // Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), null);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var partner = fixture.Freeze<Partner>(c => c.With(p => p.IsActive, false));

            var partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            var partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();

            // Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), null);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_NumberIssuedPromoCodesShouldBeZero()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var partner = fixture.Freeze<Partner>(c =>
                c.With(p => p.IsActive, true).With(p => p.PartnerLimits, CreatePartnerPromoCodeLimitsList()));

            var partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            var partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();

            // Act
            await partnersController.SetPartnerPromoCodeLimitAsync(
                Guid.NewGuid(),
                fixture.Create<SetPartnerPromoCodeLimitRequest>());

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_CancelDateShouldBeCurrentDate()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var dateTime = new DateTime();
            fixture.Freeze<Mock<IDateTimeProvider>>().Setup(d => d.CurrentDateTime).Returns(dateTime);

            var partnerPromoCodeLimits = CreatePartnerPromoCodeLimitsList();
            var partner = fixture.Freeze<Partner>(c => c.
                With(p => p.IsActive, true).
                With(p => p.PartnerLimits, partnerPromoCodeLimits));

            fixture.Freeze<Mock<IRepository<Partner>>>().
                Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).
                ReturnsAsync(partner);

            var partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();

            var activeLimit = partnerPromoCodeLimits.First();

            // Act
            await partnersController.SetPartnerPromoCodeLimitAsync(
                Guid.NewGuid(), 
                fixture.Create<SetPartnerPromoCodeLimitRequest>());

            // Assert
            activeLimit.CancelDate.Should().Be(dateTime);
        }
        
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_RequestHasLimitLessOrEqualZero_ReturnsBadRequest()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var partnerPromoCodeLimits = CreatePartnerPromoCodeLimitsList();
            var partner = fixture.Freeze<Partner>(c => c.
                With(p => p.IsActive, true).
                With(p => p.PartnerLimits, partnerPromoCodeLimits));

            fixture.Freeze<Mock<IRepository<Partner>>>().
                Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).
                ReturnsAsync(partner);

            var partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();

            var request = fixture.Build<SetPartnerPromoCodeLimitRequest>().With(r => r.Limit, 0).Create();
            
            // Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(
                Guid.NewGuid(), 
                request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }
        
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_NewLimit_ShouldAddToDatabase()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var partnerPromoCodeLimitCollectionMock = fixture.Freeze<Mock<ICollection<PartnerPromoCodeLimit>>>();
            var partner = fixture.Freeze<Partner>(c => c.
                With(p => p.IsActive, true).
                With(p => p.PartnerLimits, partnerPromoCodeLimitCollectionMock.Object));

            var repositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).
                ReturnsAsync(partner);

            var partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();

            var request = fixture.Build<SetPartnerPromoCodeLimitRequest>().
                With(r => r.Limit, 1).
                Create();
            
            // Act
            await partnersController.SetPartnerPromoCodeLimitAsync(
                Guid.NewGuid(), 
                request);

            // Assert
            partnerPromoCodeLimitCollectionMock.Verify(
                c => c.Add(It.IsAny<PartnerPromoCodeLimit>()),
                Times.Once);
            repositoryMock.Verify(r => r.UpdateAsync(partner), Times.Once);
        }

        private static List<PartnerPromoCodeLimit> CreatePartnerPromoCodeLimitsList()
        {
            return
            [
                new()
                {
                    Limit = 100,
                    EndDate = DateTime.Now.AddDays(1)
                }
            ];
        }
    }
}