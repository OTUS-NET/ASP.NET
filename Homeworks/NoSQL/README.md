# Otus.PromoCodeFactory

Проект для домашних заданий и демо по курсу `C# ASP.NET Core Разработчик` от `Отус`.
Cистема `Promocode Factory` для выдачи промокодов партнеров для клиентов по группам предпочтений.


Данный проект является стартовой точкой для домашнего задания по NoSQL.

Инструкция по запуску:
Проект состоит из трех микросервисов и их баз данных, настройка Posgress баз для них приведена в docker-compose файле в корне репозитория, чтобы запустить только базы данных выполняем команду:
docker-compose up promocode-factory-administration-db promocode-factory-receiving-from-partner-db promocode-factory-giving-to-customer-db
Сами сервисы доступны в общем solution: Otus.Teaching.Pcf.sln
Если базы данных запущены, то в Visual Studio или Rider настраиваем запуск нескольких проектов сразу и работаем с API через Swagger, для API в Swagger добавлены примеры данных для вызова и тестирования.