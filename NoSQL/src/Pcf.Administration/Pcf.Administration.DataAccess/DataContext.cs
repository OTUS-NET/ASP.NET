using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess
{
    public class DataContext
        : DbContext
    {
        public DbSet<Role> Roles { get; set; }        
        public DbSet<Employee> Employees { get; set; }

        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().ToCollection("roles");
            modelBuilder.Entity<Employee>().ToCollection("employees");
        }
    }
}