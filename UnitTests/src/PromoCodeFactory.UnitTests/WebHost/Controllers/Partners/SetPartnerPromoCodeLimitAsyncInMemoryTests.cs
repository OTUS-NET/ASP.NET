using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.UnitTests.Helps;
using PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.Fixtures;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Helpers;
using PromoCodeFactory.WebHost.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncInMemoryTest: IClassFixture<Fixture_InMemory>
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly PartnersController _partnersController;
        public SetPartnerPromoCodeLimitAsyncInMemoryTest(Fixture_InMemory memoryFixture)
        {
            var serviceProvider = memoryFixture.ServiceProvider;
            _partnerRepository = serviceProvider.GetService<IPartnerRepository>();
            _partnersController = new PartnersController(_partnerRepository);
        }

        [Fact]
        public async Task SetPartnerCodeLimitAsync_SettingNewLimits_SuccessfullySavedInDataBase()
        {
            //Arrange
            Fixture autoFixture = new Fixture();
            autoFixture.Customize<SetPartnerPromoCodeLimitRequest>(c => c.FromFactory(new PartnerPromoCodeLimitBuilder()));

            var limit = autoFixture.Create<SetPartnerPromoCodeLimitRequest>();
            var partnerId = FakeDataFactory.Partners[0].Id;
            var limitBuilder = new PartnerPromoCodeLimitBuilder();
            //Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, limit);
            //Assert
            var modifyPartner =await _partnerRepository.GetByIdAsync(partnerId);
            modifyPartner.PartnerLimits.Last().Limit.Should().Be(limit.Limit);
            modifyPartner.PartnerLimits.Last().EndDate.Should().Be(limit.EndDate);
        }
    }
}
