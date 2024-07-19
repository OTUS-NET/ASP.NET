using Microsoft.EntityFrameworkCore;
using Pcf.Administration.DataAccess;

namespace Pcf.Administration.IntegrationTests
{
    public class TestDataContext
        : DataContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=PromocodeFactoryAdministrationDb.sqlite");
        }
    }
}