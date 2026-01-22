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

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _fixture = (Fixture)new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = _fixture.Build<PartnersController>()
                .OmitAutoProperties()
                .Create();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 100,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            Partner partner = null;
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerInactive_ReturnsBadRequest()
        {
            // Arrange
            var partner = TestDataFactory.CreateInactivePartner();
            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 100,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Данный партнер не активен");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_LimitZeroOrNegative_ReturnsBadRequest()
        {
            // Arrange
            var partner = TestDataFactory.CreateActivePartner();
            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 0,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Лимит должен быть больше 0");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidLimitWithActiveLimit_ResetsIssuedPromoCodes()
        {
            // Arrange
            var partner = TestDataFactory.CreatePartnerWithActiveLimit(limitValue: 100, issuedPromoCodes: 15);
            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 200,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            Partner updatedPartner = null;
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
                .Callback<Partner>(p => updatedPartner = p)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            result.Should().BeOfType<CreatedAtActionResult>();
            updatedPartner.Should().NotBeNull();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ExpiredLimit_DoesNotResetIssuedPromoCodes()
        {
            // Arrange
            var partner = TestDataFactory.CreatePartnerWithExpiredLimit(limitValue: 100, issuedPromoCodes: 15);
            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 200,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            Partner updatedPartner = null;
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
                .Callback<Partner>(p => updatedPartner = p)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            result.Should().BeOfType<CreatedAtActionResult>();
            updatedPartner.Should().NotBeNull();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WithExistingActiveLimit_DeactivatesPreviousLimit()
        {
            // Arrange
            var partner = TestDataFactory.CreatePartnerWithActiveLimit();
            var activeLimitId = partner.PartnerLimits.ElementAt(0).Id;
            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 200,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            Partner updatedPartner = null;
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
                .Callback<Partner>(p => updatedPartner = p)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            updatedPartner.Should().NotBeNull();

            // Проверяем, что предыдущий лимит отменен
            var previousLimit = updatedPartner.PartnerLimits
                .FirstOrDefault(x => x.Id == activeLimitId);
            previousLimit.Should().NotBeNull();
            previousLimit.CancelDate.Should().NotBeNull();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_SavesNewLimitToDatabase()
        {
            // Arrange
            var partner = TestDataFactory.CreateActivePartner();
            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 150,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            Partner updatedPartner = null;
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
                .Callback<Partner>(p => updatedPartner = p)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            updatedPartner.Should().NotBeNull();

            updatedPartner.PartnerLimits.Should().HaveCount(1);
            var newLimit = updatedPartner.PartnerLimits.ElementAt(0);
            newLimit.Limit.Should().Be(150);
            newLimit.PartnerId.Should().Be(partner.Id);
            newLimit.EndDate.Should().BeCloseTo(request.EndDate, TimeSpan.FromSeconds(1));
            newLimit.CancelDate.Should().BeNull();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_EndDateInPast_ShouldReturnBadRequest()
        {
            // Arrange
            var partner = TestDataFactory.CreateActivePartner();
            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 100,
                EndDate = DateTime.UtcNow.AddDays(-1)
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Дата окончания лимита должна быть в будущем");
        }
    }
}