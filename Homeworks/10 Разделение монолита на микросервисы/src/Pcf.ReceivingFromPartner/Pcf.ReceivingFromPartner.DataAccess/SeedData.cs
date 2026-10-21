using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.DataAccess;

internal static class SeedData
{
    public static IReadOnlyCollection<Preference> Preferences { get; } =
    [
        new Preference { Id = Guid.Parse("ef7f299f-92d7-459f-a716-22d27e0a8f86"), Name = "Театр" },
        new Preference { Id = Guid.Parse("c4bda62e-fc74-4256-a956-476641b679d2"), Name = "Семинары" },
        new Preference { Id = Guid.Parse("76324d47-68b2-4d88-ae67-4dc65a2f3e1a"), Name = "Кинопремьеры" }
    ];

    public static IReadOnlyCollection<Partner> Partners { get; } =
    [
        new Partner
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"),
            Name = "Театр им. Чехова",
            IsActive = true,
            ManagerId = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895")
        },
        new Partner
        {
            Id = Guid.Parse("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"),
            Name = "Центр обучения",
            IsActive = true,
            ManagerId = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895")
        }
    ];

    public static IReadOnlyCollection<PartnerPromoCodeLimit> PartnerPromoCodeLimits { get; } =
    [
        new PartnerPromoCodeLimit
        {
            Id = Guid.Parse("c1d2e3f4-a5b6-4c7d-8e9f-0a1b2c3d4e5f"),
            Partner = Partners.ElementAt(0),
            Limit = 100,
            IssuedCount = 5,
            CreatedAt = DateTimeOffset.Now.AddDays(-10),
            EndAt = DateTimeOffset.Now.AddDays(20)
        },
        new PartnerPromoCodeLimit
        {
            Id = Guid.Parse("d2e3f4a5-b6c7-4d8e-9f0a-1b2c3d4e5f6a"),
            Partner = Partners.ElementAt(1),
            Limit = 50,
            IssuedCount = 0,
            CreatedAt = DateTimeOffset.Now.AddDays(-5),
            EndAt = DateTimeOffset.Now.AddDays(55)
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
            Partner = Partners.ElementAt(0),
            Preference = Preferences.ElementAt(0)
        },
        new PromoCode
        {
            Id = Guid.Parse("e9a1b2c3-d4e5-4f6a-b7c8-d9e0f1a2b3c4"),
            Code = "SEMINAR",
            ServiceInfo = "Скидка на семинары",
            BeginDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(100),
            Partner = Partners.ElementAt(1),
            Preference = Preferences.ElementAt(1)
        }
    ];
}
