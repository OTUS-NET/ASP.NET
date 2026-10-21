using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess;

internal static class SeedData
{
    public static IReadOnlyCollection<Preference> Preferences { get; } =
    [
        new Preference { Id = Guid.Parse("ef7f299f-92d7-459f-a716-22d27e0a8f86"), Name = "Театр" },
        new Preference { Id = Guid.Parse("c4bda62e-fc74-4256-a956-476641b679d2"), Name = "Семинары" },
        new Preference { Id = Guid.Parse("76324d47-68b2-4d88-ae67-4dc65a2f3e1a"), Name = "Кинопремьеры" }
    ];

    public static IReadOnlyCollection<Customer> Customers { get; } =
    [
        new Customer
        {
            Id = Guid.Parse("a3f767aa-1918-4b0d-a3c9-37e5a0e5f3b2"),
            FirstName = "Иван",
            LastName = "Иванов",
            Email = "ivan@example.com",
            Preferences = [Preferences.ElementAt(0), Preferences.ElementAt(1)]
        },
        new Customer
        {
            Id = Guid.Parse("b4e878bb-2029-4c1e-b4d0-48f6b1f6a4c3"),
            FirstName = "Мария",
            LastName = "Петрова",
            Email = "maria@example.com",
            Preferences = [Preferences.ElementAt(1), Preferences.ElementAt(2)]
        }
    ];

    public static IReadOnlyCollection<PromoCode> PromoCodes { get; } =
    [
        new PromoCode
        {
            Id = Guid.Parse("d8f0a1b2-c3d4-4e5f-a6b7-c8d9e0f1a2b3"),
            Code = "THEATER",
            ServiceInfo = "Скидка на театральные билеты",
            BeginDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(100),
            PartnerId = Guid.Parse("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"),
            Preference = Preferences.ElementAt(0)
        },
        new PromoCode
        {
            Id = Guid.Parse("e9a1b2c3-d4e5-4f6a-b7c8-d9e0f1a2b3c4"),
            Code = "SEMINAR",
            ServiceInfo = "Скидка на семинары",
            BeginDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(100),
            PartnerId = Guid.Parse("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"),
            Preference = Preferences.ElementAt(1)
        }
    ];

    public static IReadOnlyCollection<CustomerPromoCode> CustomerPromoCodes { get; } =
    [
        new CustomerPromoCode
        {
            Id = Guid.Parse("f1a2b3c4-d5e6-4f7a-b8c9-d0e1f2a3b4c5"),
            CustomerId = Guid.Parse("a3f767aa-1918-4b0d-a3c9-37e5a0e5f3b2"),
            PromoCodeId = Guid.Parse("d8f0a1b2-c3d4-4e5f-a6b7-c8d9e0f1a2b3"),
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            AppliedAt = null
        },
        new CustomerPromoCode
        {
            Id = Guid.Parse("a2b3c4d5-e6f7-4a8b-c9d0-e1f2a3b4c5d6"),
            CustomerId = Guid.Parse("b4e878bb-2029-4c1e-b4d0-48f6b1f6a4c3"),
            PromoCodeId = Guid.Parse("e9a1b2c3-d4e5-4f6a-b7c8-d9e0f1a2b3c4"),
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            AppliedAt = DateTimeOffset.Now
        }
    ];
}
