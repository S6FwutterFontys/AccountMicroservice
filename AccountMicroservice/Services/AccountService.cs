using System;
using System.Threading.Tasks;
using AccountMicroservice.Domain;
using AccountMicroservice.Exceptions;
using AccountMicroservice.Helpers;
using AccountMicroservice.Models;
using AccountMicroservice.Repositories;
using MessageBroker;
using Newtonsoft.Json;

namespace AccountMicroservice.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly IMessageQueuePublisher _messageQueuePublisher;
        
        private readonly IHasher _hasher;
        private readonly IRegexHelper _regexHelper;
        private readonly IJWTTokenGenerator _tokenGenerator;

        public AccountService(IAccountRepository repository, IHasher hasher, IJWTTokenGenerator tokenGenerator, IRegexHelper regexHelper, IMessageQueuePublisher messageQueuePublisher)
        {
            _repository = repository;
            _hasher = hasher;
            _tokenGenerator = tokenGenerator;
            _regexHelper = regexHelper;
            _messageQueuePublisher = messageQueuePublisher;
        }

        public async Task<Account> Login(LoginModel loginModel)
        {
            var account = _repository.Get(loginModel.Email).Result;
            if (account == null) 
                throw new ArgumentException("A user with this email address does not exist. ");
            
            if (!await _hasher.VerifyHash(loginModel.Password, account.Salt, account.Password))
                throw new ArgumentException("The password is incorrect.");

            account.Token = _tokenGenerator.GenerateJWT(account.Id);
            return account.WithoutPassword();
        }

        public async Task<Account> CreateAccount(CreateAccountModel accountModel)
        {
            var acc = await _repository.Get(accountModel.Email);
            
            if (acc != null)
                throw new EmailAlreadyExistsException(); 
            
            if(!_regexHelper.IsValidEmail(accountModel.Email))
                throw new ArgumentException("Email is not a valid email.");
            
            if (!_regexHelper.IsValidPassword(accountModel.Password))
                throw new ArgumentException("Password doesn't meet the requirements.");
            
            var salt = _hasher.CreateSalt();
            var hashedPassword = await _hasher.HashPassword(accountModel.Password, salt);
            
            var newAccount = await _repository.Create(new Account()
            {
                Id = Guid.NewGuid(),
                Username = accountModel.Username,
                Email = accountModel.Email,
                Password = hashedPassword,
                Salt = salt
            });

            await _messageQueuePublisher.PublishMessageAsync("fwutter-exchange", "EmailMicroserviceQueue", "RegisterUser",
                JsonConvert.SerializeObject(new object[]
                {
                    newAccount.Id,
                    newAccount.Email,
                    newAccount.Username
                }));

            return newAccount.WithoutPassword();
        }

        public async Task<Account> UpdateAccount(Guid userId, UpdateAccountModel updateAccountModel)
        {
            if(_regexHelper.IsValidEmail(updateAccountModel.Email)) throw new ArgumentException("New email is incorrect");

            var account = await GetAccount(userId);
            account.Email = updateAccountModel.Email;

            var updatedAccount = await _repository.Update(userId, account);
            return updatedAccount.WithoutPassword();
        }

        public async Task<Account> UpdatePassword(Guid accountId, UpdatePasswordModel updatedPasswordModel)
        {
            var account = await GetAccountWithEncryptedPassword(accountId);
            
            if (!await _hasher.VerifyHash(updatedPasswordModel.OldPassword, account.Salt, account.Password))
            {
                throw new ArgumentException("The password is incorrect.");
            }

            if (_regexHelper.IsValidPassword(updatedPasswordModel.NewPassword))
            {
                //hash the password. 
                var salt = _hasher.CreateSalt();
                var hashedPassword = await _hasher.HashPassword(updatedPasswordModel.NewPassword, salt);
                account.Salt = salt;
                account.Password = hashedPassword;
            }
            
            await _repository.Update(account.Id, account);
            var updatedAccount = await _repository.Get(accountId);
            
            return updatedAccount.WithoutPassword();
        }

        public async Task<Account> GetAccount(Guid userId)
        {
            var account = await _repository.Get(userId);
            return account.WithoutPassword();
        }

        public async Task DeleteAccount(Guid userId)
        {
            if (await _repository.Get(userId) != null)
            {
                await _repository.Remove(userId);
            }
        }
        
        private async Task<Account> GetAccountWithEncryptedPassword(Guid id)
        {
            var account = await _repository.Get(id);
            return account;
        }
    }
}