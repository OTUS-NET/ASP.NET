using AwesomeAssertions;
using Bogus;
using Microsoft.AspNetCore.Http;
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
/// Тесты для <see cref="PartnersController.CreateLimit"/>.
/// Проверяемые сценарии: партнер не найден (404), партнер заблокирован (422),
/// успешное создание лимита (201), отмена предыдущих активных лимитов
/// и обработка <see cref="EntityNotFoundException"/> при обновлении партнера (404).
/// </summary>
public class SetLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerLimitsRepositoryMock;
    private readonly PartnersController _sut;

    /// <summary>
    /// 
    /// </summary>
    public SetLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerLimitsRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();

        // SUT — тестируемый контроллер.
        _sut = new PartnersController(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = new Faker().Random.Guid();
        var request = CreateValidRequest();

        // Репозиторий не находит партнера: GetById возвращает null.
        // Обратите внимание на флаги: контроллер запрашивает партнера вместе с навигационными
        // свойствами (withIncludes: true), поэтому мок настроен именно на такую перегрузку.
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;

        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        // 404 должен содержать заполненный ProblemDetails, а не пустое тело ответа.
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {partnerId} not found.");

        // Побочные эффекты недопустимы: ни обновления партнера, ни создания лимита.
        _partnersRepositoryMock.Verify(
            r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = new Faker().Random.Guid();
        var request = CreateValidRequest();
        var partner = CreatePartner(partnerId, isActive: false);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        var unprocessableResult = (UnprocessableEntityObjectResult)result.Result!;

        unprocessableResult.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);

        unprocessableResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)unprocessableResult.Value!;
        problemDetails.Title.Should().Be("Partner blocked");
        problemDetails.Detail.Should().Be("Cannot create limit for a blocked partner.");

        // Заблокированному партнеру лимит не создается и партнер не изменяется.
        _partnersRepositoryMock.Verify(
            r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        // Arrange
        var partnerId = new Faker().Random.Guid();
        var request = CreateValidRequest();

        // Активный партнер без единого лимита: отменять нечего, поэтому ветка Update не выполняется.
        var partner = CreatePartner(partnerId, isActive: true);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Запоминаем момент времени до вызова, чтобы проверить, что CreatedAt проставлен "сейчас".
        var beforeAct = DateTimeOffset.UtcNow;

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtResult = (CreatedAtActionResult)result.Result!;

        createdAtResult.StatusCode.Should().Be(StatusCodes.Status201Created);

        // Location должен указывать на действие GetLimit, а в маршрут попадать id нового лимита.
        createdAtResult.ActionName.Should().Be(nameof(PartnersController.GetLimit));
        createdAtResult.RouteValues.Should().NotBeNull();
        ((Guid)createdAtResult.RouteValues!["partnerId"]!).Should().Be(partnerId);

        createdAtResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();
        var response = (PartnerPromoCodeLimitResponse)createdAtResult.Value!;

        ((Guid)createdAtResult.RouteValues!["limitId"]!).Should().Be(response.Id);
        response.Id.Should().NotBe(Guid.Empty);
        response.EndAt.Should().Be(request.EndAt);
        response.Limit.Should().Be(request.Limit);
        response.IssuedCount.Should().Be(0);
        response.CanceledAt.Should().BeNull();
        response.CreatedAt.Should().BeCloseTo(beforeAct, TimeSpan.FromMinutes(1));

        // Лимит сохранен через репозиторий лимитов и связан с найденным партнером.
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(
                It.Is<PartnerPromoCodeLimit>(l =>
                    l.Partner == partner
                    && l.EndAt == request.EndAt
                    && l.Limit == request.Limit
                    && l.IssuedCount == 0
                    && l.CanceledAt == null),
                It.IsAny<CancellationToken>()),
            Times.Once);

        // Активных лимитов не было — партнера обновлять не требовалось.
        _partnersRepositoryMock.Verify(
            r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        // Arrange
        var partnerId = new Faker().Random.Guid();
        var request = CreateValidRequest();

        var partner = CreatePartner(partnerId, isActive: true);

        // Два действующих лимита (CanceledAt == null) — их контроллер обязан отменить.
        var firstActiveLimit = CreateLimit(partner, canceledAt: null, endAt: DateTimeOffset.UtcNow.AddDays(10));
        var secondActiveLimit = CreateLimit(partner, canceledAt: null, endAt: DateTimeOffset.UtcNow.AddDays(20));

        // Третий лимит отменен ранее: его CanceledAt меняться не должен (контроллер трогает только активные).
        var previouslyCanceledAt = DateTimeOffset.UtcNow.AddDays(-3);
        var alreadyCanceledLimit = CreateLimit(partner, canceledAt: previouslyCanceledAt);

        partner.PartnerLimits.Add(firstActiveLimit);
        partner.PartnerLimits.Add(secondActiveLimit);
        partner.PartnerLimits.Add(alreadyCanceledLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var beforeAct = DateTimeOffset.UtcNow;

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtResult = (CreatedAtActionResult)result.Result!;
        var response = (PartnerPromoCodeLimitResponse)createdAtResult.Value!;

        // Оба активных лимита отменены, причем тем же моментом времени, что и создание нового лимита.
        firstActiveLimit.CanceledAt.Should().NotBeNull();
        firstActiveLimit.CanceledAt!.Value.Should().BeCloseTo(beforeAct, TimeSpan.FromMinutes(1));
        secondActiveLimit.CanceledAt.Should().NotBeNull();
        secondActiveLimit.CanceledAt!.Value.Should().BeCloseTo(beforeAct, TimeSpan.FromMinutes(1));

        // Новый лимит создан активным (CanceledAt == null), старое значение отмененного лимита не перезаписано.
        response.CanceledAt.Should().BeNull();
        alreadyCanceledLimit.CanceledAt.Should().Be(previouslyCanceledAt);

        // Партнер обновлен ровно один раз, после чего добавлен новый лимит.
        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(
                It.Is<PartnerPromoCodeLimit>(l => l.Id == response.Id && l.Limit == request.Limit),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var partnerId = new Faker().Random.Guid();
        var request = CreateValidRequest();

        var partner = CreatePartner(partnerId, isActive: true);

        // Есть активный лимит, значит контроллер попытается обновить партнера...
        var activeLimit = CreateLimit(partner, canceledAt: null);
        partner.PartnerLimits.Add(activeLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // ...но репозиторий сообщает, что сущность уже удалена/не существует.
        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partnerId));

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        // Контроллер перехватывает EntityNotFoundException и отдает "пустой" 404 (без ProblemDetails).
        result.Result.Should().BeOfType<NotFoundResult>();

        // Новый лимит в этом сценарии не создается.
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Генерирует валидный запрос на создание лимита: дата окончания в будущем,
    /// количество промокодов больше нуля (ограничения из <see cref="PartnerPromoCodeLimitCreateRequest"/>).
    /// </summary>
    private static PartnerPromoCodeLimitCreateRequest CreateValidRequest(int? limit = null)
    {
        var faker = new Faker();

        return new PartnerPromoCodeLimitCreateRequest(
            EndAt: DateTimeOffset.UtcNow.AddDays(faker.Random.Int(1, 30)),
            Limit: limit ?? faker.Random.Int(1, 100));
    }

    /// <summary>
    /// Создает партнера с помощью AutoBogus: все обязательные поля (<see cref="Partner.Manager"/>,
    /// <see cref="Partner.Name"/>) заполняются автоматически, а поля, важные для теста, задаются явно.
    /// </summary>
    private static Partner CreatePartner(Guid partnerId, bool isActive)
    {
        // AutoBogus рекурсивно заполняет вложенные сущности, но Role нужна фиксированная ссылка,
        // чтобы не создавать лишние графы объектов.
        var role = new AutoFaker<Role>()
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .Generate();

        var manager = new AutoFaker<Employee>()
            .RuleFor(e => e.Id, _ => Guid.NewGuid())
            .RuleFor(e => e.Role, _ => role)
            .Generate();

        // Коллекция лимитов создается пустой: набор лимитов задается в каждом тесте отдельно.
        var limits = new List<PartnerPromoCodeLimit>();

        return new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.Name, _ => new Faker().Company.CompanyName())
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.Manager, _ => manager)
            .RuleFor(p => p.PartnerLimits, _ => limits)
            .Generate();
    }

    /// <summary>
    /// Создает лимит партнера с заданными датой окончания, количеством промокодов,
    /// счетчиком выданных промокодов и датой отмены.
    /// </summary>
    private static PartnerPromoCodeLimit CreateLimit(
        Partner partner,
        DateTimeOffset? canceledAt = null,
        DateTimeOffset? endAt = null,
        int? limit = null,
        int issuedCount = 0)
    {
        var faker = new Faker();

        return new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.Id, _ => faker.Random.Guid())
            .RuleFor(l => l.Partner, _ => partner)
            .RuleFor(l => l.CreatedAt, _ => DateTimeOffset.UtcNow.AddDays(-1))
            .RuleFor(l => l.EndAt, _ => endAt ?? DateTimeOffset.UtcNow.AddDays(30))
            .RuleFor(l => l.CanceledAt, _ => canceledAt)
            .RuleFor(l => l.Limit, _ => limit ?? faker.Random.Int(10, 100))
            .RuleFor(l => l.IssuedCount, _ => issuedCount)
            .Generate();
    }
}
