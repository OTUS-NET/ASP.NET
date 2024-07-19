using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.DataAccess;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    public class TestDataContext
        : DataContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=PromocodeFactoryGivingToCustomerDb.sqlite");
        }
    }
}