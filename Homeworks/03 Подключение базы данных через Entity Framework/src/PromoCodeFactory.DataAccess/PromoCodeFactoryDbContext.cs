using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess;

/// <summary>
/// Контекст базы данных приложения.
/// Описывает наборы таблиц и правила маппинга доменных моделей на SQLite через Entity Framework Core.
/// </summary>
public class PromoCodeFactoryDbContext : DbContext
{
    /// <summary>
    /// Создает контекст с настройками подключения и провайдера, которые передаются через DI.
    /// </summary>
    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Таблица сотрудников.
    /// </summary>
    public DbSet<Employee> Employees => Set<Employee>();

    /// <summary>
    /// Таблица ролей сотрудников.
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// Таблица клиентов.
    /// </summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>
    /// Таблица предпочтений клиентов.
    /// </summary>
    public DbSet<Preference> Preferences => Set<Preference>();

    /// <summary>
    /// Таблица промокодов.
    /// </summary>
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

    /// <summary>
    /// Таблица выдач промокодов клиентам.
    /// </summary>
    public DbSet<CustomerPromoCode> CustomerPromoCodes => Set<CustomerPromoCode>();

    /// <summary>
    /// Настраивает маппинг доменных моделей на таблицы базы данных:
    /// ключи, обязательные поля, ограничения длины, связи и индексы.
    /// Доменные модели при этом не изменяются.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Роль хранится отдельно, чтобы сотрудники ссылались на нее через внешний ключ.
        modelBuilder.Entity<Role>(builder =>
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
            builder.Property(r => r.Description).HasMaxLength(500);
        });

        // Вычисляемое свойство FullName не хранится в БД, потому что оно собирается из FirstName и LastName.
        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Email).HasMaxLength(256).IsRequired();
            builder.Ignore(e => e.FullName);
            builder
                .HasOne(e => e.Role)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Клиент связан с предпочтениями через many-to-many и с выданными промокодами через CustomerPromoCode.
        modelBuilder.Entity<Customer>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FirstName).HasMaxLength(50).IsRequired();
            builder.Property(c => c.LastName).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(256).IsRequired();
            builder.Ignore(c => c.FullName);
            builder
                .HasMany(c => c.Preferences)
                .WithMany(p => p.Customers);
            builder
                .HasMany(c => c.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(cpc => cpc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Предпочтение используется как категория интересов клиента и как критерий выдачи промокода.
        modelBuilder.Entity<Preference>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        });

        // Промокод связан с менеджером-партнером, предпочтением и списком клиентских выдач.
        modelBuilder.Entity<PromoCode>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Code).HasMaxLength(100).IsRequired();
            builder.Property(p => p.ServiceInfo).HasMaxLength(256).IsRequired();
            builder.Property(p => p.PartnerName).HasMaxLength(256).IsRequired();
            builder
                .HasOne(p => p.PartnerManager)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(p => p.Preference)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasMany(p => p.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(cpc => cpc.PromoCodeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Уникальный индекс запрещает повторную выдачу одного промокода одному и тому же клиенту.
        modelBuilder.Entity<CustomerPromoCode>(builder =>
        {
            builder.HasKey(cpc => cpc.Id);
            builder.HasIndex(cpc => new { cpc.CustomerId, cpc.PromoCodeId }).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
