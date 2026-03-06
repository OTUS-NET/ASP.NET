using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Repositories;
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
        private readonly IFixture _fixture;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = _fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        /// <summary>
        /// 1. Если партнер не найден, то также нужно выдать ошибку 404;
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), new());

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        /// <summary>
        /// 2. Если партнер заблокирован, то есть поле IsActive = false в классе Partner, то также нужно выдать ошибку 400;
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNoActive_ReturnsBadRequest()
        {
            // Arrange
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Partner { IsActive = false });

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), new());

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        /// <summary>
        /// 3.1 Если партнеру выставляется лимит, то мы должны обнулить количество промокодов, которые партнер выдал NumberIssuedPromoCodes
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNumberIssuedPromoCodesEqualZero_ReturnsTrue()
        {
            // Arrange
            IEnumerable<PartnerPromoCodeLimit> partnerPromoCodeLimit = _fixture
                .Build<PartnerPromoCodeLimit>()
                .With(p => p.CancelDate, () => null)
                .OmitAutoProperties()
                .CreateMany(2);

            Partner partner = _fixture.Build<Partner>()
                .With(p => p.NumberIssuedPromoCodes, 100)
                .With(p => p.PartnerLimits, partnerPromoCodeLimit.ToList())
                .Create();

            SetPartnerPromoCodeLimitRequest setPartnerPromoCodeLimitRequest = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(p => p.Limit, 1)
                .Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        /// <summary>
        /// 3.2 Если партнеру выставляется лимит, если лимит закончился, то количество не обнуляется;
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerPromoCodeLimitCancelDateNotNull_ReturnsTrue()
        {
            // Arrange
            IEnumerable<PartnerPromoCodeLimit> partnerPromoCodeLimit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(p => p.CancelDate, DateTime.UtcNow)
                .OmitAutoProperties()
                .CreateMany(2);

            Partner partner = _fixture.Build<Partner>()
                .With(p => p.NumberIssuedPromoCodes, 100)
                .With(p => p.PartnerLimits, partnerPromoCodeLimit.ToList())
                .Create();

            SetPartnerPromoCodeLimitRequest setPartnerPromoCodeLimitRequest = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(p => p.Limit, 1)
                .Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            partner.NumberIssuedPromoCodes.Should().NotBe(0);
        }

        /// <summary>
        /// 4. При установке лимита нужно отключить предыдущий лимит;
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerPreviousLimitDisabled_ReturnsTrue()
        {
            // Arrange
            IEnumerable<PartnerPromoCodeLimit> partnerPromoCodeLimit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(p => p.CancelDate, () => null)
                .OmitAutoProperties()
                .CreateMany(1);
            Partner partner = _fixture.Build<Partner>()
                .With(p => p.PartnerLimits, partnerPromoCodeLimit.ToList())
                .Create();

            SetPartnerPromoCodeLimitRequest setPartnerPromoCodeLimitRequest = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(p => p.Limit, 1)
                .Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            partner.PartnerLimits.Count.Should().BeGreaterThan(1);
            partner.PartnerLimits.ToList()[0].CancelDate.Should().NotBeNull();
        }

        /// <summary>
        /// 5.1 Лимит должен быть больше 0;
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerLimitMoreThanZero_ReturnsTrue(int limit)
        {
            // Arrange
            IEnumerable<PartnerPromoCodeLimit> partnerPromoCodeLimit = _fixture.Build<PartnerPromoCodeLimit>()
                .OmitAutoProperties()
                .CreateMany(2);

            Partner partner = _fixture.Build<Partner>()
                .With(p => p.PartnerLimits, partnerPromoCodeLimit.ToList())
                .Create();

            SetPartnerPromoCodeLimitRequest setPartnerPromoCodeLimitRequest = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(p => p.Limit, limit)
                .Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            //Assert
            partner.PartnerLimits.ToList()[partner.PartnerLimits.Count - 1].Limit.Should().BePositive();
        }

        /// <summary>
        /// 5.2 Лимит должен быть больше 0;
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerLimitLessOrEqualZero_ReturnsBadRequest(int limit)
        {
            // Arrange
            IEnumerable<PartnerPromoCodeLimit> partnerPromoCodeLimit = _fixture.Build<PartnerPromoCodeLimit>()
                .OmitAutoProperties()
                .CreateMany(2);

            Partner partner = _fixture.Build<Partner>()
                .With(p => p.PartnerLimits, partnerPromoCodeLimit.ToList())
                .Create();

            SetPartnerPromoCodeLimitRequest setPartnerPromoCodeLimitRequest = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(p => p.Limit, limit)
                .Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            //Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        // 6. Нужно убедиться, что сохранили новый лимит в базу данных(это нужно проверить Unit-тестом);
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerLimitIsSaveInDataBase_ReturnsTrue()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dataContext = new DataContext(options);

            var partnersRepository = new EfRepository<Partner>(dataContext);
            _fixture.Inject<IRepository<Partner>>(partnersRepository);
            var partnersController = _fixture.Build<PartnersController>()
                .OmitAutoProperties()
                .Create();

            IEnumerable<PartnerPromoCodeLimit> partnerPromoCodeLimit = _fixture.Build<PartnerPromoCodeLimit>()
                .OmitAutoProperties()
                .CreateMany(1);
            Partner partner = _fixture.Build<Partner>()
                .With(p => p.PartnerLimits, partnerPromoCodeLimit.ToList())
                .Create();

            await partnersRepository.AddAsync(partner);

            SetPartnerPromoCodeLimitRequest setPartnerPromoCodeLimitRequest = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(p => p.Limit, 1)
                .Create();

            // Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            Partner partnerSaved = await partnersRepository.GetByIdAsync(partner.Id);
            partnerSaved.PartnerLimits.ToList()[1].Limit.Should().Be(1);
        }
    }
}