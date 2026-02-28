using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data;

public static class PromoCodeFactoryDbSeeder
{
    public static async Task SeedAsync(PromoCodeFactoryDbContext context, CancellationToken ct)
    {
        if (await context.Roles.AnyAsync(ct))
            return;

        var roles = FakeDataFactory.Roles.ToList();
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync(ct);

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin", ct);
        var partnerManagerRole = await context.Roles.FirstAsync(r => r.Name == "PartnerManager", ct);

        var employees = new List<Employee>
        {
            new()
            {
                Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                Role = adminRole
            },
            new()
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                Role = partnerManagerRole
            }
        };
        context.Employees.AddRange(employees);
        await context.SaveChangesAsync(ct);

        var preferences = new List<Preference>
        {
            new() { Id = Guid.Parse("ef7f299f-92d7-459f-a716-22d27e0a8f86"), Name = "Театр" },
            new() { Id = Guid.Parse("c4bda62e-fc74-4256-a956-476641b679d2"), Name = "Семинары" },
            new() { Id = Guid.Parse("76324d47-68b2-4d88-ae67-4dc65a2f3e1a"), Name = "Кинопремьеры" }
        };
        context.Preferences.AddRange(preferences);
        await context.SaveChangesAsync(ct);

        var prefs = await context.Preferences.ToListAsync(ct);

        var customers = new List<Customer>
        {
            new()
            {
                Id = Guid.Parse("a3f767aa-1918-4b0d-a3c9-37e5a0e5f3b2"),
                FirstName = "Иван",
                LastName = "Иванов",
                Email = "ivan@example.com",
                Preferences = [prefs[0], prefs[1]]
            },
            new()
            {
                Id = Guid.Parse("b4e878bb-2029-4c1e-b4d0-48f6b1f6a4c3"),
                FirstName = "Мария",
                LastName = "Петрова",
                Email = "maria@example.com",
                Preferences = [prefs[1], prefs[2]]
            }
        };
        context.Customers.AddRange(customers);
        await context.SaveChangesAsync(ct);
    }
}
