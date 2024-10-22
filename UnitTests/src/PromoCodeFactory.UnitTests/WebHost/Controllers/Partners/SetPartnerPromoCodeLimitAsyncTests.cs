using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        //TODO: Add Unit Tests
        
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _partnersRepositoryMock = new Mock<IRepository<Partner>>();
            _partnersController = new PartnersController(_partnersRepositoryMock.Object);
        }
        //ИмяЕдиницыТестирования_Условие_ОжидаемыйРезультат
        //1. Если партнер не найден, то также нужно выдать ошибку 404;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync((Partner)null);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, new SetPartnerPromoCodeLimitRequest());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = false };
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, new SetPartnerPromoCodeLimitRequest());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Данный партнер не активен");
        }
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = true, PartnerLimits = new List<PartnerPromoCodeLimit>() };
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest { Limit = 100, EndDate = DateTime.Now.AddDays(30) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtActionResult = result as CreatedAtActionResult;
            createdAtActionResult.ActionName.Should().Be(nameof(PartnersController.GetPartnerLimitAsync));
            createdAtActionResult.RouteValues["id"].Should().Be(partnerId);
            createdAtActionResult.RouteValues["limitId"].Should().NotBeNull();
        }
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_InvalidLimit_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, IsActive = true, PartnerLimits = new List<PartnerPromoCodeLimit>()};
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest { Limit = 0,  EndDate = DateTime.Now };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Лимит должен быть больше 0");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActiveLimitExists_CancelsPreviousLimit()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var activeLimit = new PartnerPromoCodeLimit { Limit = 50, CancelDate = null };
            var partner = new Partner { Id = partnerId, IsActive = true, PartnerLimits = new List<PartnerPromoCodeLimit> { activeLimit } };
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest { Limit = 100, EndDate = DateTime.Now.AddDays(30) };

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            activeLimit.CancelDate.Should().NotBeNull();
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }
    }
}