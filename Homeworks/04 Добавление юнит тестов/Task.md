### Цель

Проанализировать логику работы контроллеров и добавить для них юнит тесты с помощью xUnit.  Попрактиковаться в использовании вспомогательных для тестирования библиотек: Moq, AwesomeAssertions, Bogus

---

### Описание

Ссылка на [GitHub](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/04%20Добавление%20юнит%20тестов/Task.md)

Перед выполнением нужно ознакомиться с [Правилами отправки домашнего задания на проверку](../docs/homework-rules.md)

Добавлены сущности Partner и сущность PartnerPromoCodeLimit, которая отвечает за допустимое количество промокодов (Limit), которое может выдать партнер. Добавлен PartnersController, который управляет партнерами и лимитами. При создании промокода в PromoCodesController.Create проверяется наличие лимита. Для тестов создан проект PromoCodeFactory.UnitTests. В качестве примера реализован один набор тестов для PartnersController.CancelLimit.

1. Тесты для PartnersController.CreateLimit. Файл PromoCodeFactory.UnitTests\WebHost\Controllers\Partners\SetLimitTests.cs
    - CreateLimit_WhenPartnerNotFound_ReturnsNotFound: Проверить, что если партнер не найден, то возвращается 404 с корректно заполненным ProblemDetails
    - CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity: Проверить, что если партнер заблокирован, то возвращается 422 с корректно заполненным 
    - CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit: Проверить, что лимит успешно создается и возвращается 201 с корректно заполненным CreatedAtActionResult
    - CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew: Проверить, что при создании нового лимита, старый отменяется
    - CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound: Проверить, что при Update возникает EntityNotFoundException, то возвращается 404
2. Тесты для PromoCodesController.Create. Файл PromoCodeFactory.UnitTests\WebHost\Controllers\PromoCodes\CreateTests.cs
    - Create_WhenPartnerNotFound_ReturnsNotFound: Проверить, что если партнер не найден, то возвращается 404 с корректно заполненным ProblemDetails
    - Create_WhenPreferenceNotFound_ReturnsNotFound: Проверить, что если предпочтение не найдено, то возвращается 404 с корректно заполненным ProblemDetails
    - Create_WhenNoActiveLimit_ReturnsUnprocessableEntity: Проверить, что если нет активного лимита, то возвращается 422 с корректно заполненным ProblemDetails
    - Create_WhenLimitExceeded_ReturnsUnprocessableEntity: Проверить, что если IssuedCount >= Limit, то возвращается 422 с корректно заполненным ProblemDetails
    - Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount: Проверить, что промокод создается и у лимита увеличивается IssuedCount

Требования к тестам:
1. Структура тестов AAA: Arrange, Act, Assert
1. Для мокирования зависимостей использовать Moq
2. Для генерации тестовых данных использовать Bogus и AutoBogus
3. Для проверки результатов использовать AwesomeAssertions

---

### Критерии оценивания

- Пункт 1 — 5 баллов
    - каждый тест 1 балл.
- Пункт 2 — 5 баллов
    - каждый тест 1 балл.

Для зачёта домашнего задания достаточно 8 баллов.
