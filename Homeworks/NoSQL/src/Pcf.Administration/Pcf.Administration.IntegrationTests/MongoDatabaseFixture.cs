using System;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Repositories;
using Pcf.Administration.IntegrationTests.Data;

namespace Pcf.Administration.IntegrationTests;

public class MongoDatabaseFixture : IDisposable
{
    private readonly MongoTestDbInitializer _mongoTestDbInitializer;
    public IRepository<Employee> EmployeeRepository { get; set; }
    public IRepository<Role> RoleRepository { get; set; }

    public MongoDatabaseFixture()
    {
        _mongoTestDbInitializer = new MongoTestDbInitializer();
        _mongoTestDbInitializer.InitializeDb();
        
        RoleRepository = new MongoRepository<Role>(_mongoTestDbInitializer.RolesCollection);
        EmployeeRepository = new MongoRepository<Employee>(_mongoTestDbInitializer.EmployeesCollection);
    }

    public void Dispose()
    {
        _mongoTestDbInitializer.CleanDb();
    }
}