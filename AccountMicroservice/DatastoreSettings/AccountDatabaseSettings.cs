﻿namespace AccountMicroservice.DatastoreSettings
{
    public class AccountDatabaseSettings : IAccountDatabaseSettings
    {
        public string AccountCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IAccountDatabaseSettings
    {
        string AccountCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}