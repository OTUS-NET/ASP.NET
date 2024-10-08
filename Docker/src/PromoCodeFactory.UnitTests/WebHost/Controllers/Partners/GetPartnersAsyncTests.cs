using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.Helps;
using PromoCodeFactory.WebHost.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YamlDotNet.Serialization;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class GetPartnersAsyncTests
    {
        [Theory, AutoMoqData]
        public async Task GetPartnersAsync_GettingCollectionOfPartners_ReturnsOk(IEnumerable<Partner> partners, [Frozen] Mock<IPartnerRepository> partnerRepositoryMock, PartnersController partnersController)
        {            
            //Arrange
            partnerRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(partners);
            //Act
            var result = await partnersController.GetPartnersAsync();
            //Assert
            result.Result.Should().BeAssignableTo<OkObjectResult>();
        }
    }
}
