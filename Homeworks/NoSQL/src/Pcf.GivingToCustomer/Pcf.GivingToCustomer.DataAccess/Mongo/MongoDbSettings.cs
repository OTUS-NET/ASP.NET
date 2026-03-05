namespace Pcf.GivingToCustomer.DataAccess.Mongo
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CustomersCollectionName { get; set; } = "customers";

        public string PreferencesCollectionName { get; set; } = "preferences";

        public string PromoCodesCollectionName { get; set; } = "promocodes";
    }
}

