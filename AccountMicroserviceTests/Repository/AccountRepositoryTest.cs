using System;
using System.Text;
using AccountMicroservice.DatastoreSettings;
using AccountMicroservice.Domain;
using AccountMicroservice.Repositories;
using Mongo2Go;
using Xunit;

namespace AccountMicroserviceTests.Repository
{
    public class AccountRepositoryTest
    {
        private readonly IAccountRepository _accountRepository;
        private readonly MongoDbRunner _mongoDbRunner;
        
        public AccountRepositoryTest()
        {
            _mongoDbRunner = MongoDbRunner.Start();
            var settings = new AccountDatabaseSettings()
            {
                ConnectionString = _mongoDbRunner.ConnectionString,
                DatabaseName = "IntergrationTest",
                AccountCollectionName = "TestCollection"
            };
            _accountRepository = new AccountRepository(settings);
        }

        [Fact]
        public async void CreateAccountTest()
        {
            var testAccount1 = new Account
            {
                Email = "test@account1.nl",
                Password = Encoding.ASCII.GetBytes("MyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };
            var testAccount2 = new Account
            {
                Email = "test@account2.nl",
                Password = Encoding.ASCII.GetBytes("AlsoMyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };
            
            //Act
            var result1 = await _accountRepository.Create(testAccount1);
            var result2 = await _accountRepository.Create(testAccount2);

            //Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(testAccount1.Email, result1.Email);
            Assert.Equal(testAccount2.Email, result2.Email);
        }
        
        [Fact]
        public async void GetByIdTest()
        {
            //Arrange
            var testAccount1 = new Account
            {
                Email = "test@account1.nl",
                Password = Encoding.ASCII.GetBytes("MyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };
            var testAccount2 = new Account
            {
                Email = "test@account2.nl",
                Password = Encoding.ASCII.GetBytes("AlsoMyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };

            //Act
            var result1 = await _accountRepository.Create(testAccount1);
            var result2 = await _accountRepository.Create(testAccount2);

            var result = await _accountRepository.Get(testAccount1.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(testAccount1.Id, result.Id);
            Assert.Equal(testAccount1.Email, result.Email);
            Assert.NotEqual(testAccount2.Id, result.Id);
        }
        
        [Fact]
        public async void GetByInvalidEmptyGuidTest()
        {
            var guid = Guid.Empty;
            //Arrange
            var testAccount1 = new Account
            {
                Email = "test@account1.nl",
                Password = Encoding.ASCII.GetBytes("MyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };
            var testAccount2 = new Account
            {
                Email = "test@account2.nl",
                Password = Encoding.ASCII.GetBytes("AlsoMyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };

            //Act
            await _accountRepository.Create(testAccount1);
            await _accountRepository.Create(testAccount2);

            var result = await _accountRepository.Get(guid);

            //Assert
            Assert.Null(result);
        }
           [Fact]
        public async void RemoveByIdTest()
        {
            //Arrange
            var testAccount1 = new Account
            {
                Email = "test@account1.nl",
                Password = Encoding.ASCII.GetBytes("MyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };
            var testAccount2 = new Account
            {
                Email = "test@account2.nl",
                Password = Encoding.ASCII.GetBytes("AlsoMyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };

            //Act
            await _accountRepository.Create(testAccount1);
            await _accountRepository.Create(testAccount2);

            await _accountRepository.Remove(testAccount2.Id);
            
            var result1 = await _accountRepository.Get(testAccount1.Id);
            var result2 = await _accountRepository.Get(testAccount2.Id);

            //Assert
            Assert.Null(result2);
            Assert.NotNull(result1);
        }

        [Fact]
        public async void RemoveByIdNotFoundTest()
        {
            //Arrange
            var testAccount1 = new Account
            {
                Email = "test@account1.nl",
                Password = Encoding.ASCII.GetBytes("MyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };
            var testAccount2 = new Account
            {
                Email = "test@account2.nl",
                Password = Encoding.ASCII.GetBytes("AlsoMyVeryStrongTestPassword123!"),
                Salt = new byte[] {0x20, 0x20, 0x20, 0x20}
            };

            //Act
            await _accountRepository.Create(testAccount1);
            await _accountRepository.Create(testAccount2);

            await _accountRepository.Remove(Guid.Empty);
            var result1 = await _accountRepository.Get(testAccount1.Id);
            var result2 = await _accountRepository.Get(testAccount2.Id);
            //Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
        }

    }
}