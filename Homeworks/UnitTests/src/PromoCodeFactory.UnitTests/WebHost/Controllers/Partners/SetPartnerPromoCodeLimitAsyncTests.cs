using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

/// <summary>
/// Требования
/// 1. Использовать именование: ИмяЕдиницыТестирования_Условие_ОжидаемыйРезультат;
/// 2. Для Arrange этапа должен использоваться фабричный метод при определении данных;
/// 3. Для Stub и Mock использовать Moq;
/// 4. Для создания тестируемого класса, например, PartnersController можно использовать AutoFixture(https://habr.com/ru/post/262435/), чтобы избежать ошибок компиляции при добавлении новых зависимостей в конструктор или также использовать фабричный метод;
/// 5. Для проверки утверждения в тестах должен использоваться FluentAssertions;
/// 6. В качестве дополнительного условия тестовые данные должны формироваться с помощью Builder, если не будет использоваться AutoFixture, то Mock и Stub также должны настраиваться через Builder;
/// 7. В качестве дополнительного условия можно провести рефакторинг PartnersController или только метода SetPartnerPromoCodeLimitAsync на свое усмотрение, может быть ввести дополнительные классы для повышения удобства тестирования и сопровождения, если появятся дополнительные классы и методы, то они тоже должны быть протестированы, принцип расположения тестов в проекте соответствующий тестам в примере(Например, WebHost/Controllers/PartnersControllerTests); Если будет сделан рефакторинг, то при сдаче ДЗ поясните какие классы были введены;
/// 8. В качестве дополнительного условия можно добавить прогон тестов CI и при сдаче ДЗ прислать ссылку на прогон.
/// </summary>
public class SetPartnerPromoCodeLimitAsyncTests
{
    private PartnersController _partnersController;
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly IFixture _fixture;
    private Mock<ILogger<PartnersController>> loggerMock = new Mock<ILogger<PartnersController>>();

    /// <summary>
    /// Помещаем в конструктор все самое необходимое
    /// </summary>
    public SetPartnerPromoCodeLimitAsyncTests ()
    {
        //создание "фабрики тестовых данных"
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        //"Замораживаем" мок репозитория 
        _partnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
        //создание контроллера с зависимостями
        _partnersController = _fixture.Build<PartnersController>()
            .OmitAutoProperties()
            .Create();
    }

    /// <summary>
    /// 1. Если партнер не найден, то также нужно выдать ошибку 404;
    /// </summary>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitRequest = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
        _partnersRepositoryMock.Setup(x => x.GetByIdAsync(partnerId)).ReturnsAsync((Partner)null);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, limitRequest);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// 2. Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerIsLocked_ReturnsBadRequest()
    {
        //Arrange
        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            NumberIssuedPromoCodes = 5,
            IsActive = false,
            PartnerLimits = new List<PartnerPromoCodeLimit>()
        };
        var limitRequest = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
        _partnersRepositoryMock.Setup(x => x.GetByIdAsync(partner.Id)).ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, limitRequest);

        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    /// <summary>
    /// 3.1. Если партнеру выставляется лимит, то мы должны обнулить количество промокодов, 
    /// которые партнер выдал NumberIssuedPromoCodes, если лимит закончился, то количество не обнуляется;
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_ResetsPromoCodeCounter()
    {
        //Arrange
        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            NumberIssuedPromoCodes = 100,
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>
            {
                new PartnerPromoCodeLimit 
                {
                    Id = Guid.NewGuid(),
                    CancelDate = null, 
                    Limit = 100
                }
            }
        };

        var limitRequest = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
        _partnersRepositoryMock.Setup(x => x.GetByIdAsync(partner.Id)).ReturnsAsync(partner);

        //Act
        await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, limitRequest);

        //Assert
        Assert.Equal(0, partner.NumberIssuedPromoCodes);
    }

    /// <summary>
    /// 3.2. Если партнеру выставляется лимит, то мы должны обнулить количество промокодов, 
    /// которые партнер выдал NumberIssuedPromoCodes, если лимит закончился, то количество не обнуляется;
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerHasNotActiveLimit_NotResetsPromoCodeCounter()
    {
        //Arrange
        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            NumberIssuedPromoCodes = 100,
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>
            {
                new PartnerPromoCodeLimit
                {
                    Id = Guid.NewGuid(),
                    CancelDate = DateTime.Now.AddDays(-1),
                    Limit = 100
                }
            }
        };

        var limitRequest = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
        _partnersRepositoryMock.Setup(x => x.GetByIdAsync(partner.Id)).ReturnsAsync(partner);

        //Act
        await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, limitRequest);

        Assert.Equal(100, partner.NumberIssuedPromoCodes);
    }

    /// <summary>
    /// 4. При установке лимита нужно отключить предыдущий лимит;
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_CancelsPreviousLimit()
    {
        //Arrange
        var previousLimit = new PartnerPromoCodeLimit
        {
            Id = Guid.NewGuid(),
            CancelDate = null,
            Limit = 100
        };

        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            NumberIssuedPromoCodes = 35,
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>{previousLimit}
        };

        var limitRequest = _fixture.Create<SetPartnerPromoCodeLimitRequest>();
        _partnersRepositoryMock.Setup(x => x.GetByIdAsync(partner.Id)).ReturnsAsync(partner);

        //Act
        await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, limitRequest);
        
        //Assert
        Assert.NotNull(previousLimit.CancelDate);
    }

    /// <summary>
    /// 5. Лимит должен быть больше 0;
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_SetLimit_CancelsPreviousLimit()
    {
        //Arrange
        var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>().With(x => x.Limit, -2).Create();

        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            NumberIssuedPromoCodes = 35,
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>()
        };

        _partnersRepositoryMock.Setup(x => x.GetByIdAsync(partner.Id)).ReturnsAsync(partner);

        //Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    /// <summary>
    /// 6. Нужно убедиться, что сохранили новый лимит в базу данных (это нужно проверить Unit-тестом);
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_SavesNewLimitToDatabase()
    {
        //Arrange
        var limitRequest = _fixture.Create<SetPartnerPromoCodeLimitRequest>();

        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            NumberIssuedPromoCodes = 35,
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>()
        };

        _partnersRepositoryMock.Setup(x => x.GetByIdAsync(partner.Id)).ReturnsAsync(partner);

        //Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, limitRequest);

        // Assert
        _partnersRepositoryMock.Verify(partners => partners.UpdateAsync(partner), Times.Once());
        result.Should().BeOfType<CreatedAtActionResult>();
    }
}