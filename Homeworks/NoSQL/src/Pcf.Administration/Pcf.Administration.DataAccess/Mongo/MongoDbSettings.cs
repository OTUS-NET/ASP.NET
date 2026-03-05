namespace Pcf.Administration.DataAccess.Mongo
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string EmployeesCollectionName { get; set; } = "employees";

        public string RolesCollectionName { get; set; } = "roles";
    }
}

