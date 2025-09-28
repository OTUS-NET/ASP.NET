using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.UnitTests.WebHost.Helpers;
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
        private readonly Mock<IRepository<Partner>> _repository;
        private readonly PartnersController _controller;
        private readonly SetPartnerPromoCodeLimitRequest _request;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repository = fixture.Freeze<Mock<IRepository<Partner>>>();
            _controller = fixture.Build<PartnersController>().OmitAutoProperties().Create();
            _request = fixture.Create<SetPartnerPromoCodeLimitRequest>();
        }

        /// <summary>
        /// Если партнер не найден, то также нужно выдать ошибку 404;
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            Partner partner = null;
            _repository.Setup(p => p.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, _request);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        /// <summary>
        /// Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partner = new PartnerBuilder().SetActive(false).Build();

            _repository.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, _request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        /// <summary>
        /// Если партнеру выставляется лимит, то мы должны обнулить количество промокодов, которые партнер выдал NumberIssuedPromoCodes, 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_IfLimitIsSet_NumberIssuePromoCodesShoulByZero()
        {
            // Arrange
            var partner = new PartnerBuilder().
                SetActive(true).
                SetNumberPromocode(2).
                IsSetCancelDate(false).
                Build();

            _repository.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, _request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        /// <summary>
        /// Если партнеру выставляется лимит, а лимит закончился, то количество не обнуляется
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_IfLimitHasExpired_AmountWillNotBeReset()
        {
            // Arrange
            var partner = new PartnerBuilder().
                SetActive(true).
                SetNumberPromocode(2).
                IsSetCancelDate(true).
                Build();

            _repository.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, _request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().NotBe(0);
        }

        /// <summary>
        /// При установке лимита нужно отключить предыдущий лимит;
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_IfLimitIsSet_DisablePreviousLimit()
        {
            // Arrange
            var partner = new PartnerBuilder().
                SetActive(true).
                SetNumberPromocode(2).
                IsSetCancelDate(false).
                Build();

            _repository.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            var oldDateLimit = partner.PartnerLimits.FirstOrDefault()?.CancelDate;

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, _request);

            // Assert
            partner.PartnerLimits.FirstOrDefault()?.CancelDate.Should().NotBe(oldDateLimit);
        }

        /// <summary>
        /// Лимит должен быть больше 0;
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_IfLimitLessThanZero_ReturnsBadRequest()
        {
            // Arrange
            _request.Limit = 0;

            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().
                            SetActive(true).
                            SetNumberPromocode(2).
                            IsSetCancelDate(true).
                            Build();

            _repository.Setup(p => p.GetByIdAsync(partnerId)).ReturnsAsync(partner);
            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, _request);
            Partner partnerDB = null;
            _repository.Setup(p => p.GetByIdAsync(partnerId)).ReturnsAsync(partnerDB);
            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        /// <summary>
        /// Нужно убедиться, что сохранили новый лимит в базу данных (это нужно проверить Unit-тестом);
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_IfLimitIsSet_SavedNewLimitToDB()
        {
            // Arrange
            _request.Limit = 10;

            var partnerId = Guid.NewGuid();
            Partner partner = new PartnerBuilder().
                            SetId(partnerId).
                            SetActive(true).
                            SetNumberPromocode(2).
                            IsSetCancelDate(true).
                            Build();

            _repository.Setup(p => p.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            Partner savedPartner = null;
            _repository.Setup(p => p.UpdateAsync(It.IsAny<Partner>()))
                .Callback<Partner>(p => savedPartner = p)
                .Returns(Task.CompletedTask);
            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, _request);
            // Assert
            savedPartner.Should().NotBeNull();
            var savedLimit = savedPartner.PartnerLimits.Last();
            savedLimit.Limit.Should().Be(_request.Limit);
        }
    }
}