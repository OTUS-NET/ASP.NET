### Цель

Разделение монолита на микросервисы и интеграция через HTTP API.

---

### Описание

Ссылка на [GitHub](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/10%20Разделение%20монолита%20на%20микросервисы/Task.md)

Перед выполнением нужно ознакомиться с [Правилами отправки домашнего задания на проверку](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/docs/homework-rules.md)

Роли микросервисов:

- **Pcf.Administration** — сотрудники и роли. `GET api/v1/employees/{id}` возвращает сотрудника вместе с ролью. Роль `PartnerManager` — партнерский менеджер.
- **Pcf.GivingToCustomer** — клиенты и выдача им промокодов. `POST api/v1/promocodes` сохраняет промокод и выдаёт его клиентам с указанным предпочтением.
- **Pcf.ReceivingFromPartner** — партнёры и промокоды от партнёров. При создании и изменении партнёра проверяет менеджера в Administration. После создания промокода передаёт его в GivingToCustomer.

Контракты уже заданы интерфейсами `IAdministrationGateway` и `IGivingToCustomerGateway`. Базовые адреса лежат в `IntegrationSettings` файла `Pcf.ReceivingFromPartner.WebHost/appsettings.json`: `AdministrationApiUrl` и `GivingToCustomerApiUrl`.

0. Ознакомиться с новой архитектурной проекта и изучить разделение на микросервисы.
1. Интеграция с Administration
   - В `Pcf.ReceivingFromPartner.Integration` реализовать `IAdministrationGateway`: `GET api/v1/employees/{employeeId}`.
   - Если получен ответ 404 или роль, отличная от `PartnerManager`, — сотрудник не является партнерским менеджером. Остальные неуспешные ответы запроса завершают операцию ошибкой.
   - Зарегистрировать в `AddIntegration`. Адрес сервиса брать из `IntegrationSettings:AdministrationApiUrl`. Если значение не задано, регистрация должна завершаться ошибкой.
   - В `PartnersController` принимать `IAdministrationGateway`. В `Create` и `Update` до сохранения проверять менеджера. Если проверка не пройдена, вернуть `400` с `ProblemDetails` и заголовком `Invalid manager`, партнёра не сохранять.
2. Интеграция с GivingToCustomer
   - В `Pcf.ReceivingFromPartner.Integration` реализовать `IGivingToCustomerGateway`: `POST api/v1/promocodes`.
   - В теле передать `Id`, `Code`, `ServiceInfo`, `BeginDate`, `EndDate`, `PartnerId` и `PreferenceId` созданного промокода. Неуспешный ответ завершает операцию ошибкой.
   - Зарегистрировать в `AddIntegration`. Адрес сервиса брать из `IntegrationSettings:GivingToCustomerApiUrl`. Если значение не задано, регистрация должна завершаться ошибкой.
   - В `PromoCodesController.Create` после сохранения промокода и увеличения `IssuedCount` вызвать `CreatePromoCode`.
3. docker compose
   - В `docker-compose.Development.yaml` добавить `pcf-givingtocustomer-api` и `pcf-receivingfrompartner-api` аналогично `pcf-administration-api`.
   - ReceivingFromPartner: зависит от `pcf-administration-api` и `pcf-givingtocustomer-api`, просписать строки подключения в env
   - Проверить сборку образов и запуск docker compose
4. CI
   - Обновить CI, должны собираться и публиковаться образы для каждого микросервиса.

---

### Критерии оценивания

- Пункт 1 — 3 балла
- Пункт 2 — 3 балла
- Пункт 3 — 3 балла
- Пункт 4 — 1 балл

Для зачёта домашнего задания достаточно 8 баллов.
