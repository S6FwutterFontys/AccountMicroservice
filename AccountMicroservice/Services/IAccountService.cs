using System;
using System.Threading.Tasks;
using AccountMicroservice.Domain;
using AccountMicroservice.Models;

namespace AccountMicroservice.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// Logs an account in. Returns account with JWTToken
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>Account</returns>
        Task<Account> Login(LoginModel loginModel);
        
        /// <summary>
        /// Creates an account 
        /// </summary>
        /// <param name="createAccountModel"></param>
        /// <returns>Account</returns>
        Task<Account> CreateAccount(CreateAccountModel createAccountModel);
        
        /// <summary>
        /// Gets an account without password
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Account</returns>
        Task<Account> GetAccount(Guid userId);
        
        /// <summary>
        /// Update account information
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updateAccountModel"></param>
        /// <returns>Account</returns>
        Task<Account> UpdateAccount(Guid userId, UpdateAccountModel updateAccountModel);
        
        /// <summary>
        /// Updates an account password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updatedPasswordModel"></param>
        /// <returns>AccountWithoutPassword</returns>
        Task<Account> UpdatePassword(Guid userId, UpdatePasswordModel updatedPasswordModel);
        
        /// <summary>
        /// Deletes an account 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task DeleteAccount(Guid userId);
    }
}