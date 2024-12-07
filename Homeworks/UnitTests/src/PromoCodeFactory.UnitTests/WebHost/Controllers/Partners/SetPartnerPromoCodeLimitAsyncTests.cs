using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using System;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using System.Threading.Tasks;
using System.Linq;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        //TODO: Add Unit Tests
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;
        private readonly IFixture _fixture;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = _fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        // Если партнер не найден, то также нужно выдать ошибку 404
        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            Partner partner = null;

            _partnersRepositoryMock
                .Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var request = CreateBaseRequest();
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            //Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        // Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400
        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partner = CreateBasePartner(false, false);

            _partnersRepositoryMock
                .Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var request = CreateBaseRequest();
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            ((BadRequestObjectResult)result).Value.Should().BeEquivalentTo("Данный партнер не активен");
        }

        // Если партнеру выставляется лимит, то мы должны обнулить количество промокодов, которые партнер выдал NumberIssuedPromoCodes,
        // если лимит закончился, то количество не обнуляется
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetPartnerPromoCodeLimit_CheckNumberIssuedPromoCodes_ReturnsCreatedAtActionResult(bool limitExpired)
        {
            // Arrange
            var partner = CreateActivePartner(limitExpired);
            var numberIssuedPromoCodes = partner.NumberIssuedPromoCodes;

            _partnersRepositoryMock
                .Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var request = CreateBaseRequest();
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            if (limitExpired)
                partner.NumberIssuedPromoCodes.Should().Be(numberIssuedPromoCodes);
            else
                partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        // При установке лимита нужно отключить предыдущий лимит
        [Fact]
        public async Task SetPartnerPromoCodeLimit_TurnOffLastLimit_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var partner = CreateActivePartner(false);
            var limit = partner.PartnerLimits.FirstOrDefault();

            _partnersRepositoryMock
                .Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var request = CreateBaseRequest();
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            limit.Should().NotBeNull();
            limit.CancelDate.HasValue.Should().BeTrue();
        }

        // Лимит должен быть больше 0
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SetPartnerPromoCodeLimit_RequestLimitGrt0_ReturnsBadReques(int requestLimit)
        {
            // Arrange
            var partner = CreateActivePartner(false);

            _partnersRepositoryMock
                .Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var request = CreateBaseRequest();
            request.Limit = requestLimit;
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            ((BadRequestObjectResult)result).Value.Should().BeEquivalentTo("Лимит должен быть больше 0");
        }

        // Нужно убедиться, что сохранили новый лимит в базу данных(это нужно проверить Unit-тестом)
        [Fact]
        public async Task SetPartnerPromoCodeLimit_CheckNewLimitCreated_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var partner = CreateActivePartner(false);
            var limit = partner.PartnerLimits.First();

            _partnersRepositoryMock
                .Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var request = CreateBaseRequest();
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //Assert
            limit.Should().NotBeNull();
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            _partnersRepositoryMock.Verify(r => r.UpdateAsync(partner), Times.Once);

            var newLimit = partner.PartnerLimits.FirstOrDefault(l => !l.CancelDate.HasValue);
            newLimit.Should().NotBeNull();
            newLimit.Id.Should().NotBe(limit.Id);
        }


        #region Base Objects
        private static readonly Guid _partnerId = new("7d994823-8226-4273-b063-1a95f3cc1df8");
        private static readonly Guid _partnerPromoCodeLimitId = new("e00633a5-978a-420e-a7d6-3e1dab116393");
        private Partner CreateBasePartner(bool isActive, bool limitExpired)
        {
            var limit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(l => l.Id, _partnerPromoCodeLimitId)
                .With(l => l.Limit, 10)
                .With(l => l.PartnerId, _partnerId)
                .With(l => l.CancelDate, limitExpired ? DateTime.Now : null)
                .Without(l => l.Partner);

            var partner = _fixture.Build<Partner>()
                .With(p => p.Id, _partnerId)
                .With(p => p.IsActive, isActive)
                .With(p => p.NumberIssuedPromoCodes, 5)
                .With(p => p.PartnerLimits, [limit.Create()]);

            return partner.Create();
        }
        private Partner CreateActivePartner(bool limitExpired)
        {
            return CreateBasePartner(true, limitExpired);
        }

        private SetPartnerPromoCodeLimitRequest CreateBaseRequest()
        {
            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>();
            return request.Create();
        }
        #endregion
    }
}