using System;
using Pcf.Administration.IntegrationTests.Data;

namespace Pcf.Administration.IntegrationTests
{
    public class EfDatabaseFixture: IDisposable
    {
        private readonly EfTestDbInitializer _efTestDbInitializer;
        
        public EfDatabaseFixture()
        {
            DbContext = new TestDataContext();

            _efTestDbInitializer= new EfTestDbInitializer(DbContext);
            _efTestDbInitializer.InitializeDb();
        }

        public void Dispose()
        {
            _efTestDbInitializer.CleanDb();
        }

        public TestDataContext DbContext { get; private set; }
    }
}