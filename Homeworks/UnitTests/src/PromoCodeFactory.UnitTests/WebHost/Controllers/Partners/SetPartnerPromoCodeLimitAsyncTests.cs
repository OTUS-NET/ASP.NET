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
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        private static Partner CreateBasePartner()
        {
            return new Partner()
            {
                Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                Name = "Суперигрушки",
                IsActive = true,
                NumberIssuedPromoCodes = 5,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                        PartnerId = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                        CreateDate = new DateTime(2020, 07, 9),
                        EndDate = new DateTime(2020, 10, 9),
                        Limit = 100,
                        CancelDate = null
                    }
                }
            };
        }

        private static SetPartnerPromoCodeLimitRequest CreateBaseRequest(int limit)
        {
            return new SetPartnerPromoCodeLimitRequest()
            {
                Limit = limit,
                EndDate = DateTime.Now.AddDays(30)
            };
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var request = CreateBaseRequest(limit: 10);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync((Partner)null);

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner();
            partner.IsActive = false;
            var request = CreateBaseRequest(limit: 10);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SetPartnerPromoCodeLimitAsync_LimitIsLessOrEqualZero_ReturnsBadRequest(int limit)
        {

            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner();
            var request = CreateBaseRequest(limit);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);


            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

 
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            _partnersRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Partner>()), Times.Never);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActiveLimitNotEnded_ResetsNumberIssuedPromoCodes()
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner();
            partner.PartnerLimits.Single().EndDate = DateTime.Now.AddDays(10);
            partner.NumberIssuedPromoCodes = 7;

            var request = CreateBaseRequest(limit: 42);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(partner))
                .Returns(Task.CompletedTask);

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            result.Should().BeAssignableTo<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActiveLimitEnded_DoesNotResetNumberIssuedPromoCodes()
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner();
            partner.PartnerLimits.Single().EndDate = DateTime.Now.AddDays(-10); // ended
            partner.NumberIssuedPromoCodes = 7;

            var request = CreateBaseRequest(limit: 42);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(partner))
                .Returns(Task.CompletedTask);

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            result.Should().BeAssignableTo<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(7);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ActiveLimitExists_DisablesPreviousLimit()
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner();
            var activeLimit = partner.PartnerLimits.Single();
            activeLimit.CancelDate.Should().BeNull();

            var request = CreateBaseRequest(limit: 10);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(partner))
                .Returns(Task.CompletedTask);

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            result.Should().BeAssignableTo<CreatedAtActionResult>();
            activeLimit.CancelDate.Should().NotBeNull();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_SavesNewLimitToRepository()
        {
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner();
            var request = new SetPartnerPromoCodeLimitRequest()
            {
                Limit = 123,
                EndDate = DateTime.Now.AddDays(15)
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);
            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
                .Returns(Task.CompletedTask);

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            result.Should().BeAssignableTo<CreatedAtActionResult>();

            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Partner>(p =>
                p.Id == partner.Id &&
                p.PartnerLimits.Any(l =>
                    l.Limit == request.Limit &&
                    l.EndDate == request.EndDate &&
                    l.PartnerId == partner.Id &&
                    !l.CancelDate.HasValue)
            )), Times.Once);
        }
    }
}