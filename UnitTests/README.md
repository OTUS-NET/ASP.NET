# Otus.PromoCodeFactory

Проект для домашних заданий и демо по курсу `C# ASP.NET Core Разработчик` от `Отус`.
Cистема `Promocode Factory` для выдачи промокодов партнеров для клиентов по группам предпочтений.

Для запуска проекта с базой данных `PostgreSQL` нужно использовать `docker-compose up promocode-factory-db` из корня проекта, в `Startup` файле проекта `WebHost` должна быть установлена настройка `UseNpgsql`