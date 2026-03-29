### Цель

Упаковать приложение в docker образ через Dockerfile и настроить запуск контейнера через docker compose.

---

### Описание

Ссылка на [GitHub](https://github.com/OTUS-NET/ASP.NET/blob/main/Homeworks/05%20Упаковка%20в%20docker/Task.md)

Перед выполнением нужно ознакомиться с [Правилами отправки домашнего задания на проверку](../docs/homework-rules.md)

1. Добавление Dockerfile
   - Файл Dockerfile нужно расположить в папке src
   - В Dockerfile должно быть два stage
     - build на основе mcr.microsoft.com/dotnet/sdk:10.0. Выполняет копирование файлов, restore и build
     - runtime на основе mcr.microsoft.com/dotnet/aspnet:10.0. Запускает dll, собранную в build.
2. Добавление dockerignore
   - Файл .dockerignore нужно расположить в корне репозитория
   - Минимальный набор: **/*.sqlite **/bin **/obj
3. Добавление docker compose
   - Файл docker-compose.yaml нужно расположить в корне репозитория
   - Добавить один контейнер promocode-factory-api, который должен билдить Dockerfile, созданные в пункте 1.
4. Настройка контейнера в docker compose
    - Настроить порт. Наружу должен быть доступен порт 8091
    - Добавить вольюм db и примаунтить его к контейнеру в папку /app/db
    - Настроить переменные среды:
      - Среда должна быть Development
      - sqlite БД должна использовать путь /app/db/PromoCodeFactoryDb.sqlite. Для этого нужно переопределить значение ConnectionStrings:PromocodeFactoryDb из appsettings.json

---

### Критерии оценивания

- Пункт 1 — 4 балла
- Пункт 2 — 2 балла
- Пункт 3 — 2 балла
- Пункт 4 — 2 балла

Для зачёта домашнего задания достаточно 8 баллов.
