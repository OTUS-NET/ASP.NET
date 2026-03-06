using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        //TODO: Add Unit Tests
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;
        private readonly Fixture _fixture;

        /// <summary>
        /// Конструктор
        /// </summary>
        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _partnersRepositoryMock = new Mock<IRepository<Partner>>();
            _partnersController = new PartnersController(_partnersRepositoryMock.Object);
            _fixture = new Fixture();
        }

        /// <summary>
        /// Фабричный метод для создания Partner
        /// </summary>
        /// <param name="setup">Делегат для изменения свойств экземпляра Partner</param>
        /// <returns></returns>
        private Partner BuildPartner(Action<Partner> setup = null)
        {
            var partner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = "Test Partner",
                IsActive = true,
                NumberIssuedPromoCodes = 5,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
            };
            setup?.Invoke(partner);
            return partner;
        }

        /// <summary>
        /// Фабричный метод для создания PartnerPromoCodeLimit
        /// </summary>
        /// <param name="setup">Делегат для изменения свойств экземпляра PartnerPromoCodeLimit</param>
        /// <returns></returns>
        private PartnerPromoCodeLimit BuildPartnerPromoCodeLimit(Guid partnerId,Action<PartnerPromoCodeLimit> setup = null)
        {
            var partnerPromoCodeLimir = new PartnerPromoCodeLimit
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                CreateDate = DateTime.UtcNow.AddHours(-1),
                EndDate = DateTime.UtcNow.AddMonths(1),
                Limit = 50
            };
            setup?.Invoke(partnerPromoCodeLimir);
            return partnerPromoCodeLimir;
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_Should_Throw_Exception_If_Partner_Is_NotFound_Return_404()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_Should_Throw_Exception_If_Partner_Is_NotActive_Return_400()
        {
            // Arrange
            var partner = BuildPartner(p => p.IsActive = false);
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);

            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        /// <summary>
        /// Если партнеру выставляется лимит и лимит не закончился, то количество обнуляется
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_If_Set_New_Limit_Should_Reset_NumberIssuedPromoCodes_To_Zero()
        {
            // Arrange
            var partner = BuildPartner(p => p.PartnerLimits.Add(BuildPartnerPromoCodeLimit(p.Id)));
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
            request.Limit = 100;//Выставляем новый лимит

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        /// <summary>
        /// Если партнеру выставляется лимит и лимит закончился, то количество не обнуляется
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_If_Set_NewLimit_And_PreviousLimitExpired_Should_Not_Reset_NumberIssuedPromoCodes()
        {
            // Arrange
            var partner = BuildPartner(p => p.PartnerLimits.Add(BuildPartnerPromoCodeLimit(p.Id, pl => pl.CancelDate = DateTime.UtcNow.AddDays(-1))));//Выставляем лимит промокода истекшим
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
            request.Limit = 100;//Выставляем новый лимит

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(5);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_If_Set_NewLimit_OldLimit_Should_Be_Canceled()
        {
            // Arrange
            var partner = BuildPartner(p => p.PartnerLimits.Add(BuildPartnerPromoCodeLimit(p.Id)));
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
            request.Limit = 100;//Выставляем новый лимит

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            partner.PartnerLimits.FirstOrDefault().CancelDate.Should().NotBeNull();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_Limit_Should_Not_Be_Zero_Return400()
        {
            // Arrange
            var partner = BuildPartner();
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
            request.Limit = 0;//Выставляем лимит равный нулю

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_NewLimit_Should_Be_Saved_In_DB()
        {
            // Arrange
            var partner = BuildPartner();
            _partnersRepositoryMock.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            var request = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
            request.Limit = 25;

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            _partnersRepositoryMock.Verify(r => r.UpdateAsync(partner), Times.Once);
            partner.PartnerLimits.Should().ContainSingle(x => x.Limit == request.Limit);
            result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}