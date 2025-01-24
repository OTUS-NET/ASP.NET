# Дэмо проект React + ASP.NET 8 + Docker

Разбираемся как подружить фронт на React v18 и бэк на ASP.NET 8, с размещением в Docker контейнере.
## Запуск
- `docker compose up -d`
- На хосте http://localhost:5687/ запускается приложение React v18+ API ASP.NET в одном процессе
- На хосте http://localhost:5272/ запускается отдельное приложение React, которое в качестве API использует хост http://localhost:5687/ 
- Настроен CORS для разрешения запросов между приложениями


---
React + Vite + ASP.NET + Swagger UI + Docker