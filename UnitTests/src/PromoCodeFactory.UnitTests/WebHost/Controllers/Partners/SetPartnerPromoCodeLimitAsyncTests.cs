using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly PartnersController _controller;
        private readonly Mock<IRepository<Partner>> _repository;
        private readonly Guid _partnerGuid;
        private readonly IFixture _fixture;
        //TODO: Add Unit Tests
        
        public SetPartnerPromoCodeLimitAsyncTests()
        {
            //_repository = new Mock<IRepository<Partner>>();
            //_controller = new PartnersController(_repository.Object);
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repository = _fixture.Freeze<Mock<IRepository<Partner>>>();
            _controller = _fixture.Build<PartnersController>().OmitAutoProperties().Create();
             _partnerGuid = Guid.NewGuid();

            _repository.Invocations.Clear();  // Очистка счетчика вызовов перед основным действием теста
        }

        /// <summary>
        /// Фабричный метод
        /// </summary>
        /// <returns></returns>
        private SetPartnerPromoCodeLimitRequest SetPartnerPromoCodeLimitRequestFactory()
        {
            return new SetPartnerPromoCodeLimitRequest();
        }

        //Если партнер не найден, то также нужно выдать ошибку 404;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotExist_ReturnsNotFound404()
        {
            //Arrange
            _repository.Setup(service => service.GetByIdAsync(_partnerGuid)).ReturnsAsync((Partner)null);

            //Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(_partnerGuid, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundResult>();
            result.Should().BeOfType<NotFoundResult>();
            _repository.Verify(r => r.GetByIdAsync(_partnerGuid), Times.Once);
        }

        //Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_IsActiveFalse_Returns400()
        {
            //Arrange
            var partner = _fixture.Build<Partner>().OmitAutoProperties()
                .With(x => x.Id, _partnerGuid)
                .With(x => x.IsActive, false)
                .Create();

            _repository.Setup(service => service.GetByIdAsync(_partnerGuid)).ReturnsAsync(partner);

            //Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(_partnerGuid, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            result.Should().BeOfType<BadRequestObjectResult>();
            _repository.Verify(r => r.GetByIdAsync(_partnerGuid), Times.Once);
        }


        //Если партнеру выставляется лимит, то мы должны обнулить количество промокодов,
        //которые партнер выдал NumberIssuedPromoCodes,
        //если лимит закончился, то количество не обнуляется;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_SetLimit_NumberIssuedPromoCodesZero()
        {
            //Arrange
            var partnerLimit = _fixture.Build<PartnerPromoCodeLimit>().OmitAutoProperties()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CancelDate, (DateTime?) null)
                .With(x => x.EndDate, _fixture.Create<DateTime>())
                .With(x => x.Limit, _fixture.Create<int>())
                .Do(x => x.Limit = Math.Abs(x.Limit) + 1)
                .Create();

            var partner = _fixture.Build<Partner>().OmitAutoProperties()
                .With(x => x.Id, _partnerGuid)
                .With(x => x.IsActive, true)
                .With(x => x.PartnerLimits, new List<PartnerPromoCodeLimit>() { partnerLimit })
                .Create();

            _repository.Setup(service => service.GetByIdAsync(_partnerGuid)).ReturnsAsync(partner);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>().OmitAutoProperties()
                .With(x => x.Limit, -1)
                .With(x => x.EndDate, DateTime.Now)
                .Create();

            //Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(_partnerGuid, request);

            //Assert

            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Лимит должен быть больше 0");
            partner.NumberIssuedPromoCodes.Should().Be(0);
           
            _repository.Verify(r => r.GetByIdAsync(_partnerGuid), Times.Once);
            _repository.Verify(r => r.UpdateAsync(partner), Times.Never);
        }


        //При установке лимита нужно отключить предыдущий лимит;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_SetLimit_CancelPreviousLimit()
        {
            //Arrange
            var partnerLimit = _fixture.Build<PartnerPromoCodeLimit>().OmitAutoProperties()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CancelDate, (DateTime?)null)
                .With(x => x.EndDate, _fixture.Create<DateTime>())
                .With(x => x.Limit, _fixture.Create<int>())
                .Do(x => x.Limit = Math.Abs(x.Limit) + 1)
                .Create();

            var partner = _fixture.Build<Partner>().OmitAutoProperties()
                .With(x => x.Id, _partnerGuid)
                .With(x => x.IsActive, true)
                .With(x => x.PartnerLimits, new List<PartnerPromoCodeLimit>() { partnerLimit })
                .Create();

            _repository.Setup(service => service.GetByIdAsync(_partnerGuid)).ReturnsAsync(partner);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>().OmitAutoProperties()
                .With(x => x.Limit, -1)
                .With(x => x.EndDate, DateTime.Now)
                .Create();

            //Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(_partnerGuid, request);

            //Assert

            result.Should().NotBeNull();
            partner.PartnerLimits.FirstOrDefault().CancelDate.Should().NotBeNull();
            
            _repository.Verify(r => r.GetByIdAsync(_partnerGuid), Times.Once);
            _repository.Verify(r => r.UpdateAsync(partner), Times.Never);
        }

        //Лимит должен быть больше 0;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ZeroLimit_BadRequestLimitMustBeGreaterZero()
        {
            //Arrange
            var partnerLimit = _fixture.Build<PartnerPromoCodeLimit>().OmitAutoProperties()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CancelDate, (DateTime?)null)
                .With(x => x.EndDate, _fixture.Create<DateTime>())
                .With(x => x.Limit, _fixture.Create<int>())
                .Do(x => x.Limit = Math.Abs(x.Limit) + 1)
                .Create();

            var partner = _fixture.Build<Partner>().OmitAutoProperties()
                .With(x => x.Id, _partnerGuid)
                .With(x => x.IsActive, true)
                .With(x => x.PartnerLimits, new List<PartnerPromoCodeLimit>() { partnerLimit })
                .Create();

            _repository.Setup(service => service.GetByIdAsync(_partnerGuid)).ReturnsAsync(partner);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>().OmitAutoProperties()
                .With(x => x.Limit, -1)
                .With(x => x.EndDate, DateTime.Now)
                .Create();

            //Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(_partnerGuid, request);

            //Assert

            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Лимит должен быть больше 0");

            _repository.Verify(r => r.GetByIdAsync(_partnerGuid), Times.Once);
            _repository.Verify(r => r.UpdateAsync(partner), Times.Never);
        }

        //Нужно убедиться, что сохранили новый лимит в базу данных (это нужно проверить Unit-тестом);
        //Если в текущей реализации найдутся ошибки, то их нужно исправить и желательно написать тест,
        //чтобы они больше не повторялись.
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_SetLimit_LimitSavedInDb()
        {
            //Arrange
            var partnerLimit = _fixture.Build<PartnerPromoCodeLimit>().OmitAutoProperties()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CancelDate, (DateTime?)null)
                .With(x => x.EndDate, _fixture.Create<DateTime>())
                .With(x => x.Limit, _fixture.Create<int>())
                .Do(x => x.Limit = Math.Abs(x.Limit) + 1)
                .Create();

            var partner = _fixture.Build<Partner>().OmitAutoProperties()
                .With(x => x.Id, _partnerGuid)
                .With(x => x.IsActive, true)
                .With(x => x.PartnerLimits, new List<PartnerPromoCodeLimit>() { partnerLimit })
                .Create();

            _repository.Setup(service => service.GetByIdAsync(_partnerGuid)).ReturnsAsync(partner);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>().OmitAutoProperties()
                .With(x => x.Limit, 5)
                .With(x => x.EndDate, DateTime.Now.AddDays(3))
                .Create();

            //Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(_partnerGuid, request);

            //Assert

            result.Should().NotBeNull();
            
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
           
            _repository.Verify(r => r.GetByIdAsync(_partnerGuid), Times.Once);
            _repository.Verify(r => r.UpdateAsync(partner), Times.Once);
        }

        //Лимит должен быть меньше или равен 10;
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ZeroLimit_BadRequestLimitMustBeLowerOrEqual10()
        {
            //Arrange
            var partnerLimit = _fixture.Build<PartnerPromoCodeLimit>().OmitAutoProperties()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CancelDate, (DateTime?)null)
                .With(x => x.EndDate, _fixture.Create<DateTime>())
                .With(x => x.Limit, _fixture.Create<int>())
                .Do(x => x.Limit = Math.Abs(x.Limit) + 1)
                .Create();

            var partner = _fixture.Build<Partner>().OmitAutoProperties()
                .With(x => x.Id, _partnerGuid)
                .With(x => x.IsActive, true)
                .With(x => x.PartnerLimits, new List<PartnerPromoCodeLimit>() { partnerLimit })
                .Create();

            _repository.Setup(service => service.GetByIdAsync(_partnerGuid)).ReturnsAsync(partner);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>().OmitAutoProperties()
                .With(x => x.Limit, 11)
                .With(x => x.EndDate, DateTime.Now)
                .Create();

            //Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(_partnerGuid, request);

            //Assert

            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Лимит должен быть меньше или равен 10");

            _repository.Verify(r => r.GetByIdAsync(_partnerGuid), Times.Once);
            _repository.Verify(r => r.UpdateAsync(partner), Times.Never);
        }
    }
}