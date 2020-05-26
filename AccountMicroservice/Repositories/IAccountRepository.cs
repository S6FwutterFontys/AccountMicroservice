using System;
using System.Threading.Tasks;
using AccountMicroservice.Domain;

namespace AccountMicroservice.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> Get(string email);
        Task<Account> Get(Guid id);
        Task<Account> Create(Account account);
        Task<Account> Update(Guid id, Account account);
        Task Remove(Guid id);
    }
}