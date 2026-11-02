### Цель

Перевести микросервисы с SQLite на PostgreSQL, MongoDB, Redis.

---

### Описание

Ссылка на [GitHub](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/11%20Подключение%20реляционных%20и%20NoSQL%20баз%20данных/Task.md)

Перед выполнением нужно ознакомиться с [Правилами отправки домашнего задания на проверку](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/docs/homework-rules.md)

1. Перевод Pcf.Administration и Pcf.ReceivingFromPartner на PostgreSQL
   - В `Pcf.Administration.DataAccess` и `Pcf.ReceivingFromPartner.DataAccess` заменить `Microsoft.EntityFrameworkCore.Sqlite` на `Npgsql.EntityFrameworkCore.PostgreSQL`. В регистрации контекста вызывать `UseNpgsql`.
   - Удалить миграции SQLite и создать миграцию заново. Снимок SQLite на PostgreSQL не переносится.
   - В `docker-compose.yaml` уже добавлен отдельный контейнер Postgres для каждого сервиса: `pcf-administration-db` и `pcf-receivingfrompartner-db`.
2. Перевод Pcf.GivingToCustomer на MongoDB через EF Core
   - В `Pcf.GivingToCustomer.DataAccess` заменить `Microsoft.EntityFrameworkCore.Sqlite` на `MongoDB.EntityFrameworkCore`. В регистрации контекста вызывать `UseMongoDB`. `MongoClient` зарегистрировать один на приложение.
   - Провайдер не поддерживает миграции и внешние ключи. Удалить миграции SQLite. При старте вызывать `Database.EnsureCreated`.
   - Коллекции: `preferences`, `customers`, `promoCodes`. Связи между коллекциями — только идентификаторы: у клиента `PreferenceIds`, у промокода `PreferenceId`. Выданные промокоды хранить вложенным массивом клиента (`OwnsMany`), отдельную коллекцию для них не заводить.
   - В `PromoCodesController.Create` искать клиентов по `PreferenceIds`, добавлять выданный промокод в массив клиента и сохранять клиента. Имена предпочтений для ответов брать из коллекции `preferences`.
   - В конструкторе контекста выставить `Database.AutoTransactionBehavior = AutoTransactionBehavior.Never`: контейнер MongoDB без replica set транзакции не поддерживает.
   - В `docker-compose.yaml` уже добавлен отдельный Mongo контейнер с именем `pcf-givingtocustomer-db`.
3. Справочник предпочтений Pcf.ReceivingFromPartner в Redis
   - Таблицу `Preferences` из PostgreSQL. В `PromoCodes` колонка `PreferenceId` остаётся без внешнего ключа.
   - Убрать `Preference` из `PromoCode`, оставить только `PreferenceId`
   - В `Pcf.ReceivingFromPartner.DataAccess` подключить `StackExchange.Redis`. `IConnectionMultiplexer` зарегистрировать один на приложение.
   - Добавить `PreferenceRedisRepository`, который реализует `IRepository<Preference` и получает `IConnectionMultiplexer`. Справочник хранить хешем `preferences`: поле — `Id`, значение — `Name`.
   - В `docker-compose.yaml` уже добавлен контейнер Redis с `appendonly yes` и `maxmemory-policy noeviction`.

Дополнительная информация:
1. Установка EF Core Tools: `dotnet tool install --global dotnet-ef`
2. Миграция Administration: `dotnet ef migrations add Initial --project src/Pcf.Administration/Pcf.Administration.DataAccess --startup-project src/Pcf.Administration/Pcf.Administration.WebHost`
3. Миграция ReceivingFromPartner: `dotnet ef migrations add Initial --project src/Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.DataAccess --startup-project src/Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost`

---

### Критерии оценивания

- Пункт 1 — 4 балла
- Пункт 2 — 3 балла
- Пункт 3 — 3 балла

Для зачёта домашнего задания достаточно 7 баллов.
