using System.Linq.Expressions;
using AwesomeAssertions;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.PromoCodes;

/// <summary>
/// Тесты для <see cref="PromoCodesController.Create"/>.
/// Проверяемые сценарии: партнер не найден (404), предпочтение не найдено (404),
/// нет активного лимита (422), лимит исчерпан (422) и успешное создание промокода (201)
/// с увеличением <see cref="PartnerPromoCodeLimit.IssuedCount"/>.
/// </summary>
public class CreateTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;
    private readonly PromoCodesController _sut;

    public CreateTests()
    {
        // Все пять зависимостей контроллера подменяются моками Moq:
        // сервис-слой отсутствует, логика целиком находится в контроллере.
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        var customerPromoCodesRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();

        // Порядок аргументов важен и совпадает с объявлением первичного конструктора контроллера.
        _sut = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            _customersRepositoryMock.Object,
            customerPromoCodesRepositoryMock.Object,
            _partnersRepositoryMock.Object,
            _preferencesRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = CreateValidRequest();

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;

        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {request.PartnerId} not found.");

        _preferencesRepositoryMock.Verify(
            r => r.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = CreateValidRequest();

        // Партнер существует (с действующим лимитом), но предпочтение не найдено.
        var partner = CreatePartner(request.PartnerId, isActive: true);
        partner.PartnerLimits.Add(CreateLimit(partner, canceledAt: null, limit: 10, issuedCount: 0));

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Контроллер запрашивает предпочтение без навигационных свойств (withIncludes по умолчанию false).
        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;

        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Preference not found");
        problemDetails.Detail.Should().Be($"Preference with Id {request.PreferenceId} not found.");

        // Промокод не создан, лимит не тронут.
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _partnersRepositoryMock.Verify(
            r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        // Arrange
        var request = CreateValidRequest();

        var partner = CreatePartner(request.PartnerId, isActive: true);

        // Лимитов два, но активных среди них нет:
        // первый — просрочен (EndAt в прошлом), второй — отменен (CanceledAt заполнен).
        var expiredLimit = CreateLimit(
            partner,
            canceledAt: null,
            endAt: DateTimeOffset.UtcNow.AddDays(-1),
            limit: 10,
            issuedCount: 1);
        var canceledLimit = CreateLimit(
            partner,
            canceledAt: DateTimeOffset.UtcNow.AddDays(-2),
            endAt: DateTimeOffset.UtcNow.AddDays(30),
            limit: 10,
            issuedCount: 1);

        partner.PartnerLimits.Add(expiredLimit);
        partner.PartnerLimits.Add(canceledLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        var preference = CreatePreference(request.PreferenceId);
        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        // Клиенты с подходящим предпочтением (контроллер запрашивает их до проверки лимита).
        SetupCustomersWithPreference([]);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        // Контроллер возвращает 422 через StatusCode(...), поэтому ожидается ObjectResult со StatusCode = 422.
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);

        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("No active limit");
        problemDetails.Detail.Should().Be("Partner has no active promo code limit.");

        // Промокод не создан, счетчики существующих лимитов не изменились.
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
        expiredLimit.IssuedCount.Should().Be(1);
        canceledLimit.IssuedCount.Should().Be(1);
    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        // Arrange
        var request = CreateValidRequest();

        var partner = CreatePartner(request.PartnerId, isActive: true);

        // Активный лимит уже исчерпан: IssuedCount == Limit (граничное условие IssuedCount >= Limit).
        var activeLimit = CreateLimit(
            partner,
            canceledAt: null,
            endAt: DateTimeOffset.UtcNow.AddDays(30),
            limit: 3,
            issuedCount: 3);

        partner.PartnerLimits.Add(activeLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreatePreference(request.PreferenceId));

        SetupCustomersWithPreference([]);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);

        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("Limit exceeded");
        problemDetails.Detail.Should().Be(
            "Cannot create promo code. Limit would be exceeded (current: 3/3).");

        // Промокод не создан, IssuedCount не увеличен.
        activeLimit.IssuedCount.Should().Be(3);
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _partnersRepositoryMock.Verify(
            r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        // Arrange
        var request = CreateValidRequest();

        var partner = CreatePartner(request.PartnerId, isActive: true);

        // Действующий лимит с запасом: 4 из 10 промокодов уже выдано.
        var activeLimit = CreateLimit(
            partner,
            canceledAt: null,
            endAt: DateTimeOffset.UtcNow.AddDays(30),
            limit: 10,
            issuedCount: 4);

        partner.PartnerLimits.Add(activeLimit);

        var preference = CreatePreference(request.PreferenceId);

        // Двое клиентов с нужным предпочтением получат промокод.
        var customers = new List<Customer>
        {
            CreateCustomer(preference),
            CreateCustomer(preference)
        };

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        SetupCustomersWithPreference(customers);

        // Перехватываем сущность, которую контроллер передает в репозиторий, для детальных проверок.
        PromoCode? addedPromoCode = null;
        _promoCodesRepositoryMock
            .Setup(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()))
            .Callback<PromoCode, CancellationToken>((promoCode, _) => addedPromoCode = promoCode)
            .Returns(Task.CompletedTask);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtResult = (CreatedAtActionResult)result.Result!;

        createdAtResult.StatusCode.Should().Be(StatusCodes.Status201Created);

        // Location указывает на GetById, в маршрут попадает id созданного промокода.
        createdAtResult.ActionName.Should().Be(nameof(PromoCodesController.GetById));
        createdAtResult.RouteValues.Should().NotBeNull();
        ((Guid)createdAtResult.RouteValues!["id"]!).Should().Be(addedPromoCode!.Id);

        createdAtResult.Value.Should().BeOfType<PromoCodeShortResponse>();
        var response = (PromoCodeShortResponse)createdAtResult.Value!;

        response.Id.Should().Be(addedPromoCode.Id);
        response.Code.Should().Be(request.Code);
        response.ServiceInfo.Should().Be(request.ServiceInfo);
        response.PartnerId.Should().Be(partner.Id);
        response.PreferenceId.Should().Be(preference.Id);
        response.BeginDate.Should().Be(request.BeginDate);
        response.EndDate.Should().Be(request.EndDate);

        // Промокод сохранен один раз и связан с партнером и предпочтением из запроса.
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Once);

        addedPromoCode.Should().NotBeNull();
        addedPromoCode!.Partner.Should().BeSameAs(partner);
        addedPromoCode.Preference.Should().BeSameAs(preference);
        addedPromoCode.Code.Should().Be(request.Code);
        addedPromoCode.ServiceInfo.Should().Be(request.ServiceInfo);
        addedPromoCode.BeginDate.Should().Be(request.BeginDate);
        addedPromoCode.EndDate.Should().Be(request.EndDate);

        // Промокод выдан каждому клиенту с указанным предпочтением, и еще не применен.
        addedPromoCode.CustomerPromoCodes.Should().HaveCount(customers.Count);
        addedPromoCode.CustomerPromoCodes
            .Select(cpc => cpc.CustomerId)
            .Should().BeEquivalentTo(customers.Select(c => c.Id));
        addedPromoCode.CustomerPromoCodes.Should().OnlyContain(cpc => cpc.PromoCodeId == addedPromoCode.Id);
        addedPromoCode.CustomerPromoCodes.Should().OnlyContain(cpc => cpc.AppliedAt == null);

        // Главная проверка сценария: счетчик выданных промокодов у активного лимита увеличен на 1.
        activeLimit.IssuedCount.Should().Be(5);
        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Генерирует валидный запрос на создание промокода с помощью Bogus.
    /// </summary>
    private static PromoCodeCreateRequest CreateValidRequest()
    {
        var faker = new Faker();

        return new PromoCodeCreateRequest(
            Code: faker.Random.AlphaNumeric(10),
            ServiceInfo: faker.Lorem.Sentence(),
            PartnerId: faker.Random.Guid(),
            BeginDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(30),
            PreferenceId: faker.Random.Guid());
    }

    /// <summary>
    /// Настраивает мок репозитория клиентов: контроллер ищет клиентов по предикату
    /// "есть предпочтение с нужным Id" и без навигационных свойств (withIncludes: false).
    /// </summary>
    private void SetupCustomersWithPreference(IReadOnlyCollection<Customer> customers)
    {
        _customersRepositoryMock
            .Setup(r => r.GetWhere(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);
    }

    /// <summary>
    /// Создает предпочтение с заданным Id; остальные поля заполняет AutoBogus.
    /// </summary>
    private static Preference CreatePreference(Guid preferenceId)
    {
        var customers = new List<Customer>();

        return new AutoFaker<Preference>()
            .RuleFor(p => p.Id, _ => preferenceId)
            .RuleFor(p => p.Name, _ => new Faker().Commerce.Categories(1).First())
            .RuleFor(p => p.Customers, _ => customers)
            .Generate();
    }

    /// <summary>
    /// Создает клиента с указанным предпочтением; данные генерируются Bogus и AutoBogus.
    /// </summary>
    private static Customer CreateCustomer(Preference preference)
    {
        var faker = new Faker();
        var preferences = new List<Preference> { preference };
        var customerPromoCodes = new List<CustomerPromoCode>();

        return new AutoFaker<Customer>()
            .RuleFor(c => c.Id, _ => faker.Random.Guid())
            .RuleFor(c => c.FirstName, _ => faker.Name.FirstName())
            .RuleFor(c => c.LastName, _ => faker.Name.LastName())
            .RuleFor(c => c.Email, _ => faker.Internet.Email())
            .RuleFor(c => c.Preferences, _ => preferences)
            .RuleFor(c => c.CustomerPromoCodes, _ => customerPromoCodes)
            .Generate();
    }

    /// <summary>
    /// Создает партнера с помощью AutoBogus: обязательные поля (Manager, Name) заполняются
    /// автоматически, а значимые для теста поля задаются явно.
    /// </summary>
    private static Partner CreatePartner(Guid partnerId, bool isActive)
    {
        var role = new AutoFaker<Role>()
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .Generate();

        var manager = new AutoFaker<Employee>()
            .RuleFor(e => e.Id, _ => Guid.NewGuid())
            .RuleFor(e => e.Role, _ => role)
            .Generate();

        // Коллекция лимитов создается пустой, конкретные лимиты добавляются в каждом тесте.
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
    /// Создает лимит партнера с заданными датой окончания, размером лимита,
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
