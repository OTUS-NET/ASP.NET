using Xunit;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    [CollectionDefinition(DbCollection)]
    public class EfDatabaseCollection : ICollectionFixture<EfDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the

        public const string DbCollection = "Database collection";
    }
}