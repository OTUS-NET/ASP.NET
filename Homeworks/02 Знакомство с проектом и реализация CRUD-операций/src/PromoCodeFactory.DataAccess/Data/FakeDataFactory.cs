using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Data;

public static class FakeDataFactory
{
    public static IReadOnlyCollection<Employee> Employees =>
    [
        new Employee()
        {
            Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
            Email = "owner@somemail.ru",
            FirstName = "Иван",
            LastName = "Сергеев",
            Roles =
            [
                Roles.First(x => x.Name == "Admin")  
            ],
            AppliedPromocodesCount = 5
        },
        new Employee()
        {
            Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
            Email = "andreev@somemail.ru",
            FirstName = "Петр",
            LastName = "Андреев",
            Roles =
            [
                Roles.First(x => x.Name == "PartnerManager")  
            ],
            AppliedPromocodesCount = 10
        },
    ];

    public static IReadOnlyCollection<Role> Roles =>
    [
        new Role()
        {
            Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
            Name = "Admin",
            Description = "Администратор",
        },
        new Role()
        {
            Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
            Name = "PartnerManager",
            Description = "Партнерский менеджер"
        }
    ];
}
