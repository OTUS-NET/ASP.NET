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
using PromoCodeFactory.UnitTests.Helpers;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers;

public class PartnerControllerTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly PartnersController _partnersController;

    public PartnerControllerTests()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        
        _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
        _partnersController = fixture.Build<PartnersController>()
            .OmitAutoProperties()
            .Create();
    }

    [Fact]
    public async Task CancelPartnerPromoCodeLimitAsync_SuccessfulCancellation_ReturnsNoContent()
    {
        // Arrange 
        var partnerId = Guid.NewGuid();
        var partner = EntityHelper.GetPartner(x =>
        {
            x.Id = partnerId;
            x.PartnerLimits.First().CancelDate = null;
        });
        
        _partnersRepositoryMock.Setup(x => x.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        _partnersRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Partner>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _partnersController.CancelPartnerPromoCodeLimitAsync(partnerId);
        
        // Assert
        result.Should().BeOfType<NoContentResult>();
        _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Once);
    }
    
    [Fact]
    public async Task CancelPartnerPromoCodeLimitAsync_NoActiveLimit_ReturnsNoContent()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = EntityHelper.GetPartner(p => 
        {
            p.Id = partnerId;
            p.PartnerLimits.First().CancelDate = DateTime.Now;
        });
    
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);
        _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _partnersController.CancelPartnerPromoCodeLimitAsync(partnerId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Once);
    }
    
    [Fact]
    public async Task CancelPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        Partner partner = null;
    
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.CancelPartnerPromoCodeLimitAsync(partnerId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _partnersRepositoryMock.Verify(repo => repo.GetByIdAsync(partnerId), Times.Once);
        _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Never);
    }
    
    [Fact]
    public async Task GetPartnersAsync_ReturnsOkWithPartners()
    {
        // Arrange
        var partners = new List<Partner>
        {
            EntityHelper.GetPartner(),
            EntityHelper.GetPartner(p => 
            {
                p.Id = Guid.NewGuid();
                p.Name = Guid.NewGuid().ToString();
            })
        };
    
        _partnersRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(partners);

        // Act
        var result = await _partnersController.GetPartnersAsync();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPartners = okResult.Value.Should().BeAssignableTo<IEnumerable<PartnerResponse>>().Subject;
        returnedPartners.Should().HaveCount(2);
        _partnersRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetPartnersAsync_ReturnsOkWithEmptyList_WhenNoPartners()
    {
        // Arrange
        var partners = new List<Partner>();
    
        _partnersRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(partners);

        // Act
        var result = await _partnersController.GetPartnersAsync();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPartners = okResult.Value.Should().BeAssignableTo<IEnumerable<PartnerResponse>>().Subject;
        returnedPartners.Should().BeEmpty();
        _partnersRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetPartnerLimitAsync_SuccessfulGet_ReturnsOkWithLimit()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var partner = EntityHelper.GetPartner(p => 
        {
            p.Id = partnerId;
            p.PartnerLimits.First().Id = limitId;
            p.PartnerLimits.First().PartnerId = partnerId;
        });
    
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.GetPartnerLimitAsync(partnerId, limitId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>().Subject;
        response.Id.Should().Be(limitId);
        response.PartnerId.Should().Be(partnerId);
        _partnersRepositoryMock.Verify(repo => repo.GetByIdAsync(partnerId), Times.Once);
    }
    
    [Fact]
    public async Task GetPartnerLimitAsync_PartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        Partner partner = null;
    
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.GetPartnerLimitAsync(partnerId, limitId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _partnersRepositoryMock.Verify(repo => repo.GetByIdAsync(partnerId), Times.Once);
    }
    
    [Fact]
    public async Task GetPartnerLimitAsync_LimitNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var partner = EntityHelper.GetPartner(p => 
        {
            p.Id = partnerId;
            p.PartnerLimits.Clear(); // У партнера нет лимитов
        });
    
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.GetPartnerLimitAsync(partnerId, limitId);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be("Limit not found");
        _partnersRepositoryMock.Verify(repo => repo.GetByIdAsync(partnerId), Times.Once);
    }
    
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_SuccessfulSet_ReturnsCreatedAtAction()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = EntityHelper.GetPartner(p => 
        {
            p.Id = partnerId;
            p.PartnerLimits.Clear();
        });
            
        var request = new SetPartnerPromoCodeLimitRequest
        {
            Limit = 100,
            EndDate = DateTime.Now.AddDays(30)
        };

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);
        _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Once);
    }
    
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerNotActive_ReturnsBadRequest()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = EntityHelper.GetPartner(p => 
        {
            p.Id = partnerId;
            p.IsActive = false;
        });
            
        var request = new SetPartnerPromoCodeLimitRequest
        {
            Limit = 100,
            EndDate = DateTime.Now.AddDays(30)
        };

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Данный партнер не активен");
        _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Never);
    }
    
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_InvalidLimit_ReturnsBadRequest()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = EntityHelper.GetPartner(p => p.Id = partnerId);
        var request = new SetPartnerPromoCodeLimitRequest
        {
            Limit = 0,
            EndDate = DateTime.Now.AddDays(30)
        };

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Лимит должен быть больше 0");
        _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Never);
    }
}