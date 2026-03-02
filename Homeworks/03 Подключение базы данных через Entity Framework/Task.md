### Цель

Добавить использование Entity Framework на базе SQLite вместо in-memory хранения данных. Реализовать EfRepository, методы контроллеров для Customer, PromoCode и Preference. Добавить Entity Framework миграции и первоначальное заполнение БД данными.

---

### Описание

Ссылка на [GitHub](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/03%20%D0%9F%D0%BE%D0%B4%D0%BA%D0%BB%D1%8E%D1%87%D0%B5%D0%BD%D0%B8%D0%B5%20%D0%B1%D0%B0%D0%B7%D1%8B%20%D0%B4%D0%B0%D0%BD%D0%BD%D1%8B%D1%85%20%D1%87%D0%B5%D1%80%D0%B5%D0%B7%20Entity%20Framework/Task.md)

Перед выполнением нужно ознакомиться с [Правилами отправки домашнего задания на проверку](../docs/homework-rules.md)

1. Подключение Entity Framework и SQLite
    - Добавить DbSet сущностей в PromoCodeFactoryDbContext
    - Настроить маппинг доменных моделей через OnModelCreating (доменные модели менять нельзя)
    - Добавить ограничения на длину для текстовых полей: FirstName/LastName — 50, Email — 256, Name — 100, Description — 500, Code/ServiceInfo/PartnerName — 100–256
    - Реализовать методы в EfRepository
    - Заменить InMemoryRepository на EfRepository в регистрации сервисов
2. Реализация контроллера CustomersController
    - Реализовать методы Get, GetById (с Preferences и CustomerPromoCodes), Create, Update, Delete
3. Реализация контроллера PreferencesController
    - Реализовать метод Get
4. Реализация контроллера PromoCodesController
    - Реализовать методы Get, GetById, Create (промокод + выдача клиентам с указанным предпочтением), Apply (отметить использование промокода клиентом)
5. Миграции
    - Создать миграцию InitialCreate


Дополнительная информация:
1. Установка EF Core Tools. dotnet tool install --global dotnet-ef
2. Добавить миграцию. dotnet ef migrations add ИмяМиграции --project PromoCodeFactory.DataAccess --startup-project PromoCodeFactory.WebHost

---

### Критерии оценивания

- Пункт 1 — 2 балла
- Пункт 2 — 2 балла
- Пункт 3 — 2 балла
- Пункт 4 — 2 балла
- Пункт 5 — 2 балла

Для зачёта домашнего задания достаточно 8 баллов.
