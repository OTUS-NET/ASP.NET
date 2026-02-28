using Microsoft.EntityFrameworkCore;

namespace PromoCodeFactory.DataAccess;

public class PromoCodeFactoryDbContext : DbContext
{
    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
    }

    //TODO: Добавить DbSet сущности

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: Добавить маппинг моделей

        base.OnModelCreating(modelBuilder);
    }
}
