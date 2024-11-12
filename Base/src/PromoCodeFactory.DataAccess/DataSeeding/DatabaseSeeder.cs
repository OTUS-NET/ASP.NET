using System;
using PromoCodeFactory.Core.Administration;
using PromoCodeFactory.Core.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Seeding;

public static class DatabaseSeeder
{
    public static Role[] GetRoles()
    {
        return
        [
            new Role
            {
                Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                Name = "Admin",
                Description = "Администратор",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                Name = "PartnerManager",
                Description = "Партнерский менеджер",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }
        ];
    }

    public static Employee[] GetEmployees()
    {
        return
        [
            new Employee
            {
                Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                AppliedPromoCodesCount = 5,
                RoleId = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new Employee
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                AppliedPromoCodesCount = 10,
                RoleId = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }
        ];
    }

    public static Preference[] GetPreferences()
    {
        return
        [
            new Preference
            {
                Id = Guid.Parse("848deaf5-b346-4f7a-b903-cdb97d274e99"),
                Name = "Скидки",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new Preference
            {
                Id = Guid.Parse("001a42d4-401e-4222-b0a3-f0b7c020bc2b"),
                Name = "Акции",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new Preference
            {
                Id = Guid.Parse("9dd84697-36d2-43f8-bdcc-1ebcd61da316"),
                Name = "Распродажи",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new Preference
            {
                Id = Guid.Parse("db29f052-9b5e-46cc-8db7-0c24f51be0a9"),
                Name = "Новинки",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }
        ];
    }
}