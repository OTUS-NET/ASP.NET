using Pcf.Administration.Core.Domain;

namespace Pcf.Administration.DataAccess;

internal static class SeedData
{
    public static IReadOnlyCollection<Role> Roles { get; } =
    [
        new Role
        {
            Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
            Name = "Admin",
            Description = "Администратор"
        },
        new Role
        {
            Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
            Name = "PartnerManager",
            Description = "Партнерский менеджер"
        }
    ];

    public static IReadOnlyCollection<Employee> Employees { get; } =
    [
        new Employee
        {
            Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
            Email = "owner@somemail.ru",
            FirstName = "Иван",
            LastName = "Сергеев",
            Role = Roles.First(x => x.Name == "Admin")
        },
        new Employee
        {
            Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
            Email = "andreev@somemail.ru",
            FirstName = "Петр",
            LastName = "Андреев",
            Role = Roles.First(x => x.Name == "PartnerManager")
        }
    ];
}
