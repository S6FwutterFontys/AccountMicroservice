using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountMicroservice.DatastoreSettings;
using AccountMicroservice.Domain;
using MongoDB.Driver;

namespace AccountMicroservice.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<Account> _accounts;

        public AccountRepository(IAccountDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _accounts = database.GetCollection<Account>(settings.AccountCollectionName);
        }

        public async Task<List<Account>> Get() =>
            await _accounts.Find(f => true).ToListAsync();
        
        public async Task<Account> Get(string email) =>
            await _accounts.Find(f => f.Email == email).FirstOrDefaultAsync();

        public async Task<Account> Get(Guid id) =>
            await _accounts.Find(f => f.Id == id).FirstOrDefaultAsync();

        public async Task<Account> Create(Account account)
        {
            await _accounts.InsertOneAsync(account);
            return account;
        }

        public async Task<Account> Update(Guid id, Account account)
        {
            await _accounts.ReplaceOneAsync(f => f == account, account);
            return account;
        }

        public async Task Remove(Guid id) =>
            await _accounts.DeleteManyAsync(f => f.Id == id);
    }
}