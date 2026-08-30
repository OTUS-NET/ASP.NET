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
/// Набор unit-тестов для метода <see cref="PromoCodesController.Create"/>.
/// Проверяет создание промокода с учетом партнера, предпочтения клиентов и активного лимита партнера.
/// Все зависимости контроллера заменены моками, чтобы тестировать только логику контроллера.
/// </summary>
public class CreateTests
{
    // Моки репозиториев изолируют контроллер от базы данных и позволяют проверять вызовы Add/Update.
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodesRepositoryMock;
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;

    // System Under Test: экземпляр контроллера, для которого выполняются все проверки.
    private readonly PromoCodesController _sut;

    // Bogus генерирует корректные значения запроса, не влияющие на конкретную проверяемую ветку.
    private readonly Faker _faker = new();

    /// <summary>
    /// Создает новый контроллер и новые моки репозиториев для каждого теста.
    /// Это исключает протекание настроек моков между разными сценариями.
    /// </summary>
    public CreateTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _customerPromoCodesRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();
        _sut = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            _customersRepositoryMock.Object,
            _customerPromoCodesRepositoryMock.Object,
            _partnersRepositoryMock.Object,
            _preferencesRepositoryMock.Object);
    }

    /// <summary>
    /// Метод: Create_WhenPartnerNotFound_ReturnsNotFound.
    /// Описание: если партнер из запроса не найден, Create должен вернуть 404 NotFound
    /// с корректным ProblemDetails и не должен создавать промокод.
    /// </summary>
    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = CreateRequest();

        // Имитируем отсутствие партнера в репозитории.
        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {request.PartnerId} not found.");

        // При отсутствии партнера контроллер не должен доходить до создания PromoCode.
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Метод: Create_WhenPreferenceNotFound_ReturnsNotFound.
    /// Описание: если партнер найден, но указанное предпочтение не найдено,
    /// Create должен вернуть 404 NotFound с корректным ProblemDetails и не должен создавать промокод.
    /// </summary>
    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = CreateRequest();
        var partner = CreatePartner(request.PartnerId, CreateActiveLimit());

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Имитируем отсутствие предпочтения, указанного в запросе.
        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Preference not found");
        problemDetails.Detail.Should().Be($"Preference with Id {request.PreferenceId} not found.");

        // Если предпочтение не найдено, промокод не создается.
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Метод: Create_WhenNoActiveLimit_ReturnsUnprocessableEntity.
    /// Описание: если у партнера нет активного лимита на выдачу промокодов,
    /// Create должен вернуть 422  с ProblemDetails и не должен создавать промокод.
    /// </summary>
    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        // Arrange
        var request = CreateRequest();

        // Лимит с заполненным CanceledAt считается неактивным, поэтому контроллер должен вернуть ошибку лимита.
        var partner = CreatePartner(
            request.PartnerId,
            CreateActiveLimit(canceledAt: DateTimeOffset.UtcNow.AddDays(-1)));
        var preference = CreatePreference(request.PreferenceId);

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        // В контроллере получение клиентов выполняется до проверки лимита, поэтому мок настраиваем явно
        _customersRepositoryMock
            .Setup(r => r.GetWhere(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("No active limit");
        problemDetails.Detail.Should().Be("Partner has no active promo code limit.");

        // Без активного лимита создание промокода запрещено.
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Метод: Create_WhenLimitExceeded_ReturnsUnprocessableEntity.
    /// Описание: если активный лимит найден, но количество выданных промокодов уже достигло Limit,
    /// Create должен вернуть 422 UnprocessableEntity и не должен создавать новый промокод.
    /// </summary>
    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        // Arrange
        var request = CreateRequest();

        // IssuedCount равен Limit, поэтому следующий промокод превысил бы разрешенный лимит партнера.
        var activeLimit = CreateActiveLimit(limit: 10, issuedCount: 10);
        var partner = CreatePartner(request.PartnerId, activeLimit);
        var preference = CreatePreference(request.PreferenceId);

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("Limit exceeded");
        problemDetails.Detail.Should().Be("Cannot create promo code. Limit would be exceeded (current: 10/10).");

        // При превышении лимита контроллер завершает работу до сохранения PromoCode.
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Метод: Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount.
    /// Описание: если партнер, предпочтение и активный лимит валидны, Create должен создать промокод,
    /// выдать его клиентам с указанным предпочтением, увеличить IssuedCount у активного лимита
    /// и вернуть 201 CreatedAtActionResult с корректной response-моделью.
    /// </summary>
    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        // Arrange
        var request = CreateRequest();
        var initialIssuedCount = 3;
        var activeLimit = CreateActiveLimit(limit: 10, issuedCount: initialIssuedCount);
        var partner = CreatePartner(request.PartnerId, activeLimit);
        var preference = CreatePreference(request.PreferenceId);

        // Клиенты с нужным предпочтением нужны, чтобы проверить создание CustomerPromoCode
        // для каждого найденного клиента.
        var customers = new[]
        {
            CreateCustomer(Guid.NewGuid(), preference),
            CreateCustomer(Guid.NewGuid(), preference)
        };

        // Через Callback сохраняем созданный контроллером PromoCode для последующих проверок доменной сущности.
        PromoCode? addedPromoCode = null;

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

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
        var createdResult = (CreatedAtActionResult)result.Result!;
        createdResult.ActionName.Should().Be(nameof(PromoCodesController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.Value.Should().BeOfType<PromoCodeShortResponse>();

        // Проверяем доменную сущность PromoCode, переданную в репозиторий:
        // поля должны соответствовать запросу, а навигационные свойства - найденным сущностям.
        addedPromoCode.Should().NotBeNull();
        addedPromoCode!.Code.Should().Be(request.Code);
        addedPromoCode.ServiceInfo.Should().Be(request.ServiceInfo);
        addedPromoCode.Partner.Should().BeSameAs(partner);
        addedPromoCode.Preference.Should().BeSameAs(preference);
        addedPromoCode.BeginDate.Should().Be(request.BeginDate.UtcDateTime);
        addedPromoCode.EndDate.Should().Be(request.EndDate.UtcDateTime);

        // Промокод должен быть выдан всем клиентам, найденным по указанному предпочтению.
        addedPromoCode.CustomerPromoCodes.Should().HaveCount(customers.Length);
        addedPromoCode.CustomerPromoCodes.Select(cpc => cpc.CustomerId)
            .Should().BeEquivalentTo(customers.Select(c => c.Id));

        // После успешного создания контроллер должен увеличить счетчик выданных промокодов на активном лимите.
        activeLimit.IssuedCount.Should().Be(initialIssuedCount + 1);

        // CreatedAtActionResult должен указывать на GetById и возвращать короткую модель созданного промокода.
        createdResult.RouteValues!["id"].Should().Be(addedPromoCode.Id);
        var response = (PromoCodeShortResponse)createdResult.Value!;
        response.Id.Should().Be(addedPromoCode.Id);
        response.Code.Should().Be(request.Code);
        response.ServiceInfo.Should().Be(request.ServiceInfo);
        response.PartnerId.Should().Be(request.PartnerId);
        response.PreferenceId.Should().Be(request.PreferenceId);

        // Проверяем, что промокод был сохранен, а партнер обновлен после инкремента IssuedCount.
        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Создает валидный запрос на создание промокода.
    /// Даты устанавливаются в будущем, а PartnerId и PreferenceId генерируются отдельно,
    /// чтобы каждый тест мог настроить соответствующие сущности в моках репозиториев.
    /// </summary>
    private PromoCodeCreateRequest CreateRequest()
    {
        return new PromoCodeCreateRequest(
            _faker.Random.AlphaNumeric(12),
            _faker.Commerce.ProductName(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(_faker.Random.Int(1, 5)),
            DateTimeOffset.UtcNow.AddDays(_faker.Random.Int(6, 30)),
            Guid.NewGuid());
    }

    /// <summary>
    /// Создает тестового партнера с указанным Id и набором лимитов.
    /// Partner.IsActive всегда true, потому что сценарии Create проверяют именно логику создания промокодов
    /// и ограничений по лимитам, а не блокировку партнера.
    /// </summary>
    private static Partner CreatePartner(Guid partnerId, params PartnerPromoCodeLimit[] limits)
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
            .RuleFor(p => p.IsActive, _ => true)
            .RuleFor(p => p.Manager, _ => employee)
            .RuleFor(p => p.PartnerLimits, _ => limits.ToList())
            .Generate();

        // Связываем лимиты с созданным партнером, чтобы активный лимит принадлежал именно объекту,
        // который возвращает partnersRepository.GetById.
        foreach (var limit in partner.PartnerLimits)
        {
            limit.Partner = partner;
        }

        return partner;
    }

    /// <summary>
    /// Создает лимит партнера с управляемыми значениями Limit, IssuedCount и CanceledAt.
    /// По умолчанию EndAt находится в будущем, поэтому лимит считается активным,
    /// если дополнительно не передать дату отмены в canceledAt.
    /// </summary>
    private static PartnerPromoCodeLimit CreateActiveLimit(
        int limit = 10,
        int issuedCount = 0,
        DateTimeOffset? canceledAt = null)
    {
        var partner = CreatePartner(Guid.NewGuid());

        return new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.Id, _ => Guid.NewGuid())
            .RuleFor(l => l.Partner, _ => partner)
            .RuleFor(l => l.EndAt, _ => DateTimeOffset.UtcNow.AddDays(30))
            .RuleFor(l => l.CreatedAt, _ => DateTimeOffset.UtcNow.AddDays(-1))
            .RuleFor(l => l.CanceledAt, _ => canceledAt)
            .RuleFor(l => l.Limit, _ => limit)
            .RuleFor(l => l.IssuedCount, _ => issuedCount)
            .Generate();
    }

    /// <summary>
    /// Создает тестовое предпочтение с заданным Id.
    /// Id должен совпадать с PreferenceId из запроса, чтобы контроллер прошел проверку существования предпочтения.
    /// </summary>
    private static Preference CreatePreference(Guid preferenceId)
    {
        return new AutoFaker<Preference>()
            .RuleFor(p => p.Id, _ => preferenceId)
            .RuleFor(p => p.Customers, _ => new List<Customer>())
            .Generate();
    }

    /// <summary>
    /// Создает тестового клиента с указанным предпочтением.
    /// Такой клиент имитирует результат customersRepository.GetWhere для выдачи созданного промокода.
    /// </summary>
    private static Customer CreateCustomer(Guid customerId, Preference preference)
    {
        return new AutoFaker<Customer>()
            .RuleFor(c => c.Id, _ => customerId)
            .RuleFor(c => c.Preferences, _ => new List<Preference> { preference })
            .RuleFor(c => c.CustomerPromoCodes, _ => new List<CustomerPromoCode>())
            .Generate();
    }
}
