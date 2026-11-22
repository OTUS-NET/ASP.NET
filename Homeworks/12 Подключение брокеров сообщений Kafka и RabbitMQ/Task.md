### Цель

Асинхронная интеграция между микросервисами через RabbitMQ. Подключение Kafka для отправки нотификаций.

---

### Описание

Ссылка на [GitHub](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/12%20Подключение%20брокеров%20сообщений%20Kafka%20и%20RabbitMQ/Task.md)

Перед выполнением нужно ознакомиться с [Правилами отправки домашнего задания на проверку](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/docs/homework-rules.md)

Сейчас `Pcf.ReceivingFromPartner` после создания промокода синхронно вызывает `Pcf.GivingToCustomer` по HTTP. Промокод уже сохранён, но клиент получает ошибку, если `GivingToCustomer` недоступен, и выдача клиентам не происходит. Эту связь нужно сделать асинхронной. `ReceivingFromPartner` отдаёт сообщение брокеру и отвечает партнёру. `GivingToCustomer` забирает его из очереди, когда готов. Если потребитель недоступен, сообщение остаётся у брокера и будет обработано после восстановления. Проверка менеджера в `Administration` остаётся HTTP: там нужен ответ в том же запросе.

`POST` создания промокода у партнёра возвращает `201`, когда брокер принял сообщение, а не когда клиенты уже получили код.

Отдельно партнёру отправляются уведомления о создании промокода, новом лимите и отмене лимита. Сервиса уведомлений в этой работе нет: сообщения только публикуются в Kafka.

Уже есть:

- `Pcf.Common.Contracts`: `CreatePromoCodeMessage`, очередь `GivingToCustomerQueues.PromoCodes`, `PartnerNotification` (`PartnerId`, `Message`), топик `NotificationTopics.Partners` (`partner-notifications`), `JsonOptions`.
- `pcf-rabbitmq`. Адрес — `RabbitMq:Url`, в compose он задан как `RabbitMq__Url`. Панель управления: `http://localhost:15672`, логин `pcf`.
- `pcf-kafka` и `pcf-kafka-ui`. Адрес — `Kafka:BootstrapServers`. Интерфейс: `http://localhost:8085`.

1. Подключение RabbitMQ в Pcf.ReceivingFromPartner
   - Для работы с RabbitMQ можно использовать NuGet-пакет `RabbitMQ.Client` или другой.
   - В `GivingToCustomerGateway` вместо `POST api/v1/promocodes` публиковать `CreatePromoCodeMessage`. Очередь долговечная, сообщение сохраняется при перезапуске брокера.
   - Адрес брать из `RabbitMq:Url`. Если значение не задано, создание шлюза завершается ошибкой. `IntegrationSettings:GivingToCustomerApiUrl` удалить.
   - `PromoCodesController.Create` по-прежнему вызывает `CreatePromoCode` после сохранения промокода и увеличения `IssuedCount`.
2. Подключение RabbitMQ в Pcf.GivingToCustomer
   - Логику создания промокода и выдачи его клиентам вынести из `PromoCodesController.Create` в `PromoCodeService` в `Pcf.GivingToCustomer.Core`. `POST api/v1/promocodes` вызывает этот сервис.
   - Потребитель очереди `GivingToCustomerQueues.PromoCodes` живёт в WebHost и передаёт сообщение в тот же сервис.
   - Доставка at-least-once: повтор с тем же `Id` не создаёт второй промокод и не выдаёт его клиенту ещё раз. Если промокод уже есть, а клиент его ещё не получал, выдача выполняется без повторного создания промокода.
   - `RabbitMq:Url` обязателен.
3. Уведомления партнёру через Kafka
   - В `Pcf.ReceivingFromPartner` реализовать `INotificationGateway` на `Confluent.Kafka`. Использовать уже описанный `PartnerNotification`. Продюсер один на приложение. Ключ сообщения — `PartnerId`. Адрес брать из `Kafka:BootstrapServers`. Если значение не задано, создание шлюза завершается ошибкой.
   - Публиковать уведомление после успешного сохранения: создание промокода (`PromoCodesController.Create`), новый лимит (`PartnersController.CreateLimit`) и отмена лимита (`PartnersController.CancelLimit`).
   - Потребителя нет. Сообщения остаются в топике для отдельного сервиса уведомлений, который в домашнее задание не входит.

---

### Критерии оценивания

- Пункт 1 — 4 балла
- Пункт 2 — 3 балла
- Пункт 3 — 3 балла

Для зачёта домашнего задания достаточно 7 баллов.
