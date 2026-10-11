using Xunit;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    [CollectionDefinition(DbCollection)]
    public class MongoDatabaseCollection : ICollectionFixture<MongoDatabaseFixture>
    {
        public const string DbCollection = "MongoDB collection";
    }
}
