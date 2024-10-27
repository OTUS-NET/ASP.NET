using Microsoft.EntityFrameworkCore;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Data;

namespace Pcf.Administration.DataAccess
{
    public class DataContext
        : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        
        public DbSet<Employee> Employees { get; set; }

        public DataContext()
        {
            
        }
        
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            //Нужно для Mongo если не настраивать репликации в монго контейнере. Ошибка SaveChanges()
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}