using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess
{
    public class DataContext : DbContext
    {

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Preference> Preferences { get; set; } = null!;
        public DbSet<PromoCode> PromoCodes { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite("Data Source=EfCore_test_db.db");


            // dotnet--project "PromoCodeFactory.DataAccess\PromoCodeFactory.DataAccess.csproj"  ef migrations add InitialMigration
        }

    }
}
