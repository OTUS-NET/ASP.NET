using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Partners;
using PromoCodeFactory.Services.Partners.Exceptions;
using Xunit;

namespace PromoCodeFactory.UnitTests.Services.Partners
{
    public class CancelPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersService _partnersService;

        public CancelPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersService = fixture.Build<PartnersService>().OmitAutoProperties().Create();
        }

        public Partner CreateBasePartner()
        {
            var partner = new Partner()
            {
                Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                Name = "Суперигрушки",
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                        CreateDate = new DateTime(2020, 07, 9),
                        EndDate = new DateTime(2020, 10, 9),
                        Limit = 100
                    }
                }
            };

            return partner;
        }
        
        [Fact]
        public async Task CancelPartnerPromoCodeLimitAsync_PartnerIsNotFound_ThrowsPartnerNotFoundException()
        {
            // Arrange
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            Partner partner = null;
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var action = async () => await _partnersService.CancelPartnerPromoCodeLimitAsync(partnerId);
 
            // Assert
            await action.Should().ThrowAsync<PartnerNotFoundException>();
        }
        
        [Fact]
        public async Task CancelPartnerPromoCodeLimitAsync_PartnerIsNotActive_ThrowsPartnerIsNotActiveException()
        {
            // Arrange
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            var partner = CreateBasePartner();
            partner.IsActive = false;
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var action = async () => await _partnersService.CancelPartnerPromoCodeLimitAsync(partnerId);
 
            // Assert
            await action.Should().ThrowAsync<PartnerIsNotActiveException>();
        }
    }
}