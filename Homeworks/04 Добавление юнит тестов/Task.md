### Цель

Добавить использование Entity Framework на базе SQLite вместо in-memory хранения данных. Реализовать EfRepository, методы контроллеров для Customer, PromoCode и Preference. Добавить Entity Framework миграции и первоначальное заполнение БД данными.

---

### Описание

Ссылка на [GitHub](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/03%20%D0%9F%D0%BE%D0%B4%D0%BA%D0%BB%D1%8E%D1%87%D0%B5%D0%BD%D0%B8%D0%B5%20%D0%B1%D0%B0%D0%B7%D1%8B%20%D0%B4%D0%B0%D0%BD%D0%BD%D1%8B%D1%85%20%D1%87%D0%B5%D1%80%D0%B5%D0%B7%20Entity%20Framework/Task.md)

Перед выполнением нужно ознакомиться с [Правилами отправки домашнего задания на проверку](../docs/homework-rules.md)

1. Подключение Entity Framework и SQLite
    - Добавить пакеты Microsoft.EntityFrameworkCore.Sqlite и Microsoft.EntityFrameworkCore.Design в проект PromoCodeFactory.DataAccess
    - Создать класс PromoCodeFactoryDbContext, наследующий DbContext
    - Настроить маппинг доменных моделей через OnModelCreating (доменные модели менять нельзя)
    - Добавить ограничения на длину для текстовых полей: FirstName/LastName — 50, Email — 256, Name — 100, Description — 500, Code/ServiceInfo/PartnerName — 100–256
    - Создать EfRepository, реализующий IRepository<T>
    - Заменить InMemoryRepository на EfRepository в регистрации сервисов
    - Использовать SQLite с connection string из appsettings.json (DefaultConnection)
2. Реализация методов контроллеров
    - CustomersController: Get (список), GetById (с Preferences и CustomerPromoCodes), Create, Update, Delete
    - PreferencesController: Get (список предпочтений)
    - PromoCodesController: Get, GetById, Create (промокод + выдача клиентам с указанным предпочтением), Apply (отметить использование промокода клиентом)
    - При необходимости добавлять общие методы в IRepository (например, GetWhere, GetById с includePaths)
3. Миграции
    - Добавить IDesignTimeDbContextFactory для корректной работы EF CLI
    - Создать миграцию InitialCreate
    - Применять миграции при старте приложения
4. Seeding (только для Development)
    - Реализовать заполнение БД тестовыми данными через UseSeeding или отдельный сидер
    - Seeding должен запускаться только в среде Development
    - Добавить Roles, Employees, Preferences, Customers из FakeDataFactory и при необходимости расширить данные


Дополнительная информация:
1. Установка EF Core Tools. dotnet tool install --global dotnet-ef
2. Добавить миграцию. dotnet ef migrations add ИмяМиграции --project PromoCodeFactory.DataAccess --startup-project PromoCodeFactory.WebHost

---

### Критерии оценивания

- Пункт 1 — 2 балла
- Пункт 2 — 2 балла
- Пункт 3 — 2 балла
- Пункт 4 — 2 балла

Для зачёта домашнего задания достаточно 8 баллов.
