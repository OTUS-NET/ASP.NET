### Цель
    


---

### Описание

Ссылка на [GitHub](https://github.com/PugachA/ASP.NET/blob/feat/refactor-1/Homeworks/02%20%D0%97%D0%BD%D0%B0%D0%BA%D0%BE%D0%BC%D1%81%D1%82%D0%B2%D0%BE%20%D1%81%20%D0%BF%D1%80%D0%BE%D0%B5%D0%BA%D1%82%D0%BE%D0%BC%20%D0%B8%20%D1%80%D0%B5%D0%B0%D0%BB%D0%B8%D0%B7%D0%B0%D1%86%D0%B8%D1%8F%20CRUD-%D0%BE%D0%BF%D0%B5%D1%80%D0%B0%D1%86%D0%B8%D0%B9/Task.md)

Перед выполнением нужно ознакомится с [Правила отправки домашнего задания на проверку](../docs/homework-rules.md)

1. Знакомство с проектом
    - Проект использует .NET 10.0, поэтому нужно убедиться, что у установлен SDK https://dotnet.microsoft.com/en-us/download/dotnet/10.0  
    - Собрать решение PromocodeFactory.sln
    - Запустить проект ProcodeFactory.WebHost
    - Проверить работу методов API через Swagger UI
2. Добавить метод GetById в EmployeesController
    - Расширить интерфейс IRepository<T>
    - Добавить EmployeeResponse в папку Models
    - Расширить Mapper
    - Реализовать метод Get в EmployeesController
    - Если Employee по Id не найден, то возвращать NotFound
3. Добавить метод Create в EmployeesController
    - Использовать подход аналогичный п.2
    - Если запрос не прошёл валидацию (не заполнены обязательные поля), то возвращать BadRequest
4. Добавить метод Update в EmployeesController
    - Использовать подход аналогичный п.2
    - Если Employee по Id не найден, то возвращать NotFound
    - Если запрос не прошёл валидацию (не заполнены обязательные поля), то возвращать BadRequest
5. Добавить метод Delete в EmployeesController
    - Использовать подход аналогичный п.2
    - Если Employee по Id не найден, то возвращать NotFound

---

### Критерии оценивания

- Пункт 1 - 2 балла
- Пункт 2 - 2 балла
- Пункт 3 - 2 балла
- Пункт 4 - 2 балла
- Пункт 5 - 2 балла

Для зачёта домашнего задания достаточно 8 баллов.
