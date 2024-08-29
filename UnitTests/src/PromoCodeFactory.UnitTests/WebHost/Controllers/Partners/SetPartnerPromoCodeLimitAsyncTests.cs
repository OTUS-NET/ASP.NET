using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.Infrastructure;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{

    public class SetPartnerPromoCodeLimitAsyncTests
    {
        [Theory, AutoMoqData]
        public async void SetPartnerPromoCodeLimitAsync_PartnerNotFound_Return404(
            Guid id,
            [Frozen] Mock<IRepository<Partner>> repositoryMock,
            [NoAutoProperties] PartnersController controller)
        {
            // Arrange
            Fixture fixture = new()
            {
                OmitAutoProperties = true
            };
            SetPartnerPromoCodeLimitRequest request = fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(i => i.Limit, 1).Create();
            repositoryMock
                .Setup(i => i.GetByIdAsync(id)).Returns(Task.FromResult<Partner>(null));

            // Act
            IActionResult result = await controller.SetPartnerPromoCodeLimitAsync(id, request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        } 

        [Theory, AutoMoqData]
        public async void SetPartnerPromoCodeLimitAsync_PartnerLocked_Return400(
            Guid id,
            [Frozen] Mock<IRepository<Partner>> repositoryMock,
            [NoAutoProperties] PartnersController controller)
        {
            // Arrange
            Fixture fixture = new()
            {
                OmitAutoProperties = true
            };
            SetPartnerPromoCodeLimitRequest request = fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(i => i.Limit, 1).Create();
            Partner partner = fixture.Build<Partner>()
                .With(i => i.IsActive, false).Create();
            repositoryMock
                .Setup(i => i.GetByIdAsync(id)).Returns(Task.FromResult(partner));

            // Act
            IActionResult result = await controller.SetPartnerPromoCodeLimitAsync(id, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory, AutoMoqData]
        public async void SetPartnerPromoCodeLimitAsync_PartnerHasActivePromocodeLimit_ResetPromocodeLimitAndAlsoDisablePreviousLimit(
            Guid id,
            SetPartnerPromoCodeLimitRequest request,
            [Frozen] Mock<IRepository<Partner>> repositoryMock,
            [NoAutoProperties] PartnersController controller)
        {
            // Arrange
            Fixture fixture = new()
            {
                OmitAutoProperties = true
            };
            var limit = fixture.Build<PartnerPromoCodeLimit>()
               .With(i => i.CancelDate, (DateTime?)null).Create();
            Partner partner = fixture.Build<Partner>()
                .With(i => i.IsActive, true)
                .With(i => i.NumberIssuedPromoCodes, 5)
                .With(i => i.PartnerLimits, new List<PartnerPromoCodeLimit> { limit }).Create();
            repositoryMock
                .Setup(i => i.GetByIdAsync(id)).Returns(Task.FromResult(partner));

            // Act
            IActionResult result = await controller.SetPartnerPromoCodeLimitAsync(id, request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
            limit.Should().NotBeNull();
        }

        [Theory, AutoMoqData]
        public async void SetPartnerPromoCodeLimitAsync_PartnerHasNotActivePromocodeLimit_NotResetPromocodeLimit(
            Guid id,
            SetPartnerPromoCodeLimitRequest request,
            [Frozen] Mock<IRepository<Partner>> repositoryMock,
            [NoAutoProperties] PartnersController controller)
        {
            // Arrange
            Fixture fixture = new()
            {
                OmitAutoProperties = true
            };
            Partner partner = fixture.Build<Partner>()
                .With(i => i.IsActive, true)
                .With(i => i.NumberIssuedPromoCodes, 5)
                .With(i => i.PartnerLimits, new List<PartnerPromoCodeLimit>()).Create();
            repositoryMock
                .Setup(i => i.GetByIdAsync(id)).Returns(Task.FromResult(partner));

            // Act
            IActionResult result = await controller.SetPartnerPromoCodeLimitAsync(id, request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(5);
        }

        [Theory, AutoMoqData]
        public async void SetPartnerPromoCodeLimitAsync_LimitAndAlsoRequestLimitLessToZero_Return400(
            Guid id,
            [Frozen]Mock<IRepository<Partner>> repositoryMock,
            [NoAutoProperties] PartnersController controller)
        {
            // Arrange
            Fixture fixture = new()
            {
                OmitAutoProperties = true
            };
            SetPartnerPromoCodeLimitRequest request = fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(i => i.Limit, 0).Create();
            Partner partner = fixture.Build<Partner>()
                .With(i => i.IsActive, true)
                .With(i => i.PartnerLimits, new List<PartnerPromoCodeLimit>()).Create();
            
            repositoryMock
                .Setup(i => i.GetByIdAsync(id)).Returns(Task.FromResult(partner));

            // Act
            IActionResult result = await controller.SetPartnerPromoCodeLimitAsync(id, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory, AutoMoqData]
        public async void SetPartnerPromoCodeLimitAsync_PartnerExistsAndNotLockedAndRequestLimitGreaterOrEqualToZero_PromocodeLimitSaved(
            Guid id,
            [Frozen] Mock<IRepository<Partner>> repositoryMock,
            [NoAutoProperties] PartnersController controller)
        {
            // Arrange
            Fixture fixture = new()
            {
                OmitAutoProperties = true
            };
            SetPartnerPromoCodeLimitRequest request = fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(i => i.Limit, 1).Create();
            Partner partner = fixture.Build<Partner>()
                .With(i => i.IsActive, true)
                .With(i => i.PartnerLimits, new List<PartnerPromoCodeLimit>()).Create();
            repositoryMock
                .Setup(i => i.GetByIdAsync(id)).Returns(Task.FromResult(partner));

            // Act
            //todo
        }


        // Дополнительные тесты, которые не указаны в ТЗ

        public async void SetPartnerPromoCodeLimitAsync_PartnerExistsAndNotLockedAndAlsoRequestLimitGreaterOrEqualToZero_Return201(
            Guid id,
            [Frozen] Mock<IRepository<Partner>> repositoryMock,
            [NoAutoProperties] PartnersController controller)
        {
            // Arrange
            Fixture fixture = new()
            {
                OmitAutoProperties = true
            };
            SetPartnerPromoCodeLimitRequest request = fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(i => i.Limit, 1).Create();
            Partner partner = fixture.Build<Partner>()
                .With(i => i.IsActive, true)
                .With(i => i.PartnerLimits, new List<PartnerPromoCodeLimit>()).Create();
            repositoryMock
                .Setup(i => i.GetByIdAsync(id)).Returns(Task.FromResult(partner));

            // Act
            IActionResult result = await controller.SetPartnerPromoCodeLimitAsync(id, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        public async void SetPartnerPromoCodeLimitAsync_PartnerLimitsIsNull_Return201(
           Guid id,
           [Frozen] Mock<IRepository<Partner>> repositoryMock,
           [NoAutoProperties] PartnersController controller)
        {
            // Arrange
            Fixture fixture = new()
            {
                OmitAutoProperties = true
            };
            SetPartnerPromoCodeLimitRequest request = fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(i => i.Limit, 1).Create();
            Partner partner = fixture.Build<Partner>()
                .With(i => i.IsActive, true)
                .With(i => i.PartnerLimits, (List<PartnerPromoCodeLimit>)null).Create();
            repositoryMock
                .Setup(i => i.GetByIdAsync(id)).Returns(Task.FromResult(partner));

            // Act
            Action action = async () => await controller.SetPartnerPromoCodeLimitAsync(id, request);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}