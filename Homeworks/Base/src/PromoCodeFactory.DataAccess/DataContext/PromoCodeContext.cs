using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using System;

namespace PromoCodeFactory.DataAccess.DataContext;
public class PromoCodeContext : DbContext
{
    public DbSet<Employee> Employee { get; set; }
    public DbSet<Role> Role { get; set; }

    public string DbPath { get; }

    public PromoCodeContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "promocode.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
