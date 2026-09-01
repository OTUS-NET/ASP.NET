using AwesomeAssertions;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.Partners;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

/// <summary>
/// Набор unit-тестов для метода <see cref="PartnersController.CreateLimit"/>.
/// Проверяет основные ветки создания лимита партнера: ошибки валидации состояния партнера,
/// успешное создание нового лимита, отмену старых активных лимитов и обработку исключения репозитория.
/// </summary>
public class SetLimitTests
{
    // Моки репозиториев позволяют проверить поведение контроллера без обращения к базе данных.
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerLimitsRepositoryMock;

    // System Under Test: контроллер, поведение которого проверяется в каждом тесте.
    private readonly PartnersController _sut;

    // Bogus используется для генерации допустимых случайных значений в request-объектах.
    private readonly Faker _faker = new();

    /// <summary>
    /// Создает общий набор зависимостей для каждого теста.
    /// xUnit создает новый экземпляр тестового класса на каждый тест, поэтому состояние моков не переиспользуется.
    /// </summary>
    public SetLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerLimitsRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();
        _sut = new PartnersController(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }

    /// <summary>
    /// Метод: CreateLimit_WhenPartnerNotFound_ReturnsNotFound.
    /// Описание: если партнер с указанным Id не найден, CreateLimit должен вернуть 404 NotFound
    /// с ProblemDetails, где заполнены Title и Detail, и не должен создавать лимит.
    /// </summary>
    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = CreateRequest();

        // Репозиторий партнеров имитирует ситуацию, когда запись отсутствует в хранилище.
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {partnerId} not found.");

        // При ошибке поиска партнера контроллер обязан завершить работу до вызова Add у репозитория лимитов.
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Метод: CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity.
    /// Описание: если партнер найден, но заблокирован, CreateLimit должен вернуть 422 ошибку 
    /// с корректным ProblemDetails и не должен добавлять новый лимит.
    /// </summary>
    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, isActive: false);
        var request = CreateRequest();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        var objectResult = (UnprocessableEntityObjectResult)result.Result!;
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("Partner blocked");
        problemDetails.Detail.Should().Be("Cannot create limit for a blocked partner.");

        // Заблокированному партнеру нельзя создавать лимиты, поэтому Add не вызывается.
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Метод: CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit.
    /// Описание: если запрос корректный и у партнера нет активных лимитов, CreateLimit должен создать новый лимит,
    /// вернуть 201 CreatedAtActionResult и заполнить route values для последующего получения созданного лимита.
    /// </summary>
    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, isActive: true);
        var request = CreateRequest();

        // Через Callback сохраняем сущность, которую контроллер передал в repository.Add,
        // чтобы проверить не только HTTP response, но и состояние созданного доменного объекта.
        PartnerPromoCodeLimit? addedLimit = null;

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Callback<PartnerPromoCodeLimit, CancellationToken>((limit, _) => addedLimit = limit)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = (CreatedAtActionResult)result.Result!;
        createdResult.ActionName.Should().Be(nameof(PartnersController.GetLimit));
        createdResult.RouteValues.Should().ContainKey("partnerId").WhoseValue.Should().Be(partnerId);
        createdResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();

        // Проверяем поля новой сущности лимита: она привязана к партнеру, имеет данные из запроса
        // и стартует с нулевым количеством выданных промокодов.
        addedLimit.Should().NotBeNull();
        addedLimit!.Partner.Should().BeSameAs(partner);
        addedLimit.EndAt.Should().Be(request.EndAt);
        addedLimit.Limit.Should().Be(request.Limit);
        addedLimit.IssuedCount.Should().Be(0);
        addedLimit.CanceledAt.Should().BeNull();

        // CreatedAtActionResult должен содержать id созданного лимита и response-модель с теми же значениями.
        createdResult.RouteValues.Should().ContainKey("limitId").WhoseValue.Should().Be(addedLimit.Id);
        var response = (PartnerPromoCodeLimitResponse)createdResult.Value!;
        response.Id.Should().Be(addedLimit.Id);
        response.EndAt.Should().Be(request.EndAt);
        response.Limit.Should().Be(request.Limit);
        response.IssuedCount.Should().Be(0);

        // Старых активных лимитов нет, значит обновлять партнера для их отмены не требуется.
        _partnersRepositoryMock.Verify(
            r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Метод: CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew.
    /// Описание: если у партнера уже есть активный лимит, CreateLimit должен отменить старый лимит,
    /// не менять уже отмененные лимиты, сохранить обновленного партнера и добавить новый лимит.
    /// </summary>
    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var activeLimitId = Guid.NewGuid();
        var alreadyCanceledAt = DateTimeOffset.UtcNow.AddDays(-2);

        // activeLimit считается активным, потому что CanceledAt == null.
        var activeLimit = CreateLimit(activeLimitId, canceledAt: null);

        // canceledLimit уже отменен, поэтому контроллер не должен перезаписывать его CanceledAt.
        var canceledLimit = CreateLimit(Guid.NewGuid(), canceledAt: alreadyCanceledAt);
        var partner = CreatePartner(partnerId, isActive: true, activeLimit, canceledLimit);
        var request = CreateRequest();
        PartnerPromoCodeLimit? addedLimit = null;

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Callback<PartnerPromoCodeLimit, CancellationToken>((limit, _) => addedLimit = limit)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();

        // Старый активный лимит должен получить дату отмены, а уже отмененный лимит должен остаться без изменений.
        activeLimit.CanceledAt.Should().NotBeNull();
        canceledLimit.CanceledAt.Should().Be(alreadyCanceledAt);

        // Новый лимит создается с параметрами из запроса.
        addedLimit.Should().NotBeNull();
        addedLimit!.Limit.Should().Be(request.Limit);
        addedLimit.EndAt.Should().Be(request.EndAt);

        // При наличии старых активных лимитов контроллер сначала сохраняет изменения партнера,
        // а затем добавляет новый лимит через отдельный репозиторий лимитов.
        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Метод: CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound.
    /// Описание: если при сохранении отмены старых активных лимитов репозиторий выбрасывает EntityNotFoundException,
    /// CreateLimit должен вернуть 404 NotFound и не должен добавлять новый лимит.
    /// </summary>
    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = CreatePartner(
            partnerId,
            isActive: true,
            CreateLimit(Guid.NewGuid(), canceledAt: null));
        var request = CreateRequest();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partnerId));

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        // Если Update партнера не прошел, создание нового лимита не должно продолжаться.
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Создает валидный request для установки лимита партнера.
    /// EndAt всегда находится в будущем, Limit всегда больше нуля, чтобы тесты проверяли бизнес-ветки контроллера,
    /// а не валидацию модели ASP.NET Core.
    /// </summary>
    private PartnerPromoCodeLimitCreateRequest CreateRequest()
    {
        return new PartnerPromoCodeLimitCreateRequest(
            DateTimeOffset.UtcNow.AddDays(_faker.Random.Int(1, 30)),
            _faker.Random.Int(1, 100));
    }

    /// <summary>
    /// Создает тестового партнера с заданным Id, признаком активности и набором лимитов.
    /// AutoFaker заполняет остальные обязательные поля доменов, а явно заданные RuleFor
    /// фиксируют значения, которые важны для конкретного сценария теста.
    /// </summary>
    private static Partner CreatePartner(
        Guid partnerId,
        bool isActive,
        params PartnerPromoCodeLimit[] limits)
    {
        var role = new AutoFaker<Role>()
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .Generate();

        var employee = new AutoFaker<Employee>()
            .RuleFor(e => e.Id, _ => Guid.NewGuid())
            .RuleFor(e => e.Role, role)
            .Generate();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.Manager, employee)
            .RuleFor(p => p.PartnerLimits, _ => limits.ToList())
            .Generate();

        // После создания партнера связываем каждый лимит обратно с этим партнером,
        // чтобы доменная модель соответствовала реальному кейсу.
        foreach (var limit in partner.PartnerLimits)
        {
            limit.Partner = partner;
        }

        return partner;
    }

    /// <summary>
    /// Создает тестовый лимит партнера с заданным Id и состоянием отмены.
    /// Если canceledAt равен null, лимит считается активным; если дата задана, лимит считается отмененным.
    /// </summary>
    private static PartnerPromoCodeLimit CreateLimit(Guid limitId, DateTimeOffset? canceledAt)
    {
        var partner = CreatePartner(Guid.NewGuid(), isActive: true);

        return new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.Id, _ => limitId)
            .RuleFor(l => l.Partner, _ => partner)
            .RuleFor(l => l.CanceledAt, _ => canceledAt)
            .RuleFor(l => l.CreatedAt, _ => DateTimeOffset.UtcNow.AddDays(-1))
            .RuleFor(l => l.EndAt, _ => DateTimeOffset.UtcNow.AddDays(30))
            .RuleFor(l => l.Limit, f => f.Random.Int(1, 100))
            .RuleFor(l => l.IssuedCount, f => f.Random.Int(0, 10))
            .Generate();
    }
}
