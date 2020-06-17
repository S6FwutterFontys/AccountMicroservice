using System;
using System.Text;
using System.Threading.Tasks;
using AccountMicroservice.Domain;
using AccountMicroservice.Helpers;
using AccountMicroservice.Models;
using AccountMicroservice.Repositories;
using AccountMicroservice.Services;
using MessageBroker;
using Moq;
using Xunit;

namespace AccountMicroserviceTests.ServiceTests
{
    public class AccountServiceTest
    {
        private readonly IAccountService _accountService;
        private readonly Mock<IMessageQueuePublisher> _messageQueuePublisher;
        private readonly Mock<IAccountRepository> _repository;
        private readonly Mock<IHasher> _hasher;
        private readonly Mock<IJWTTokenGenerator> _jwtGenerator;
        private readonly Mock<IRegexHelper> _regexHelper;

        public AccountServiceTest()
        {
            _jwtGenerator = new Mock<IJWTTokenGenerator>();
            _hasher = new Mock<IHasher>();
            _repository = new Mock<IAccountRepository>();
            _regexHelper = new Mock<IRegexHelper>();
            _messageQueuePublisher = new Mock<IMessageQueuePublisher>();
            _accountService = new AccountMicroservice.Services.AccountService(_repository.Object, _hasher.Object, _jwtGenerator.Object,
                _regexHelper.Object, _messageQueuePublisher.Object);
        }

        [Fact]
        public async Task CreateAccountSuccess()
        {
            const string email = "iemand@gmail.com";
            const string password = "MyV3rysecurepw!!2";
            var encryptedPassword = Encoding.ASCII.GetBytes(password);
            var salt = new byte[] {0x20, 0x20, 0x20, 0x20};

            var account = new Account
            {
                Email = email,
                Password = encryptedPassword,
                Salt = salt,
            };

            var createModel = new CreateAccountModel
            {
                Email = "iemand@gmail.com",
                Password = "MyV3rysecurepw!!2"
            };

            _repository.Setup(x => x.Get(createModel.Email)).ReturnsAsync((Account) null);
            _hasher.Setup(x => x.CreateSalt()).Returns(salt);
            _hasher.Setup(x => x.HashPassword(password, salt)).ReturnsAsync(encryptedPassword);
            _repository.Setup(x => x.Create(It.IsAny<Account>())).ReturnsAsync(account);
            _regexHelper.Setup(x => x.IsValidEmail(createModel.Email)).Returns(true);
            _regexHelper.Setup(x => x.IsValidPassword(createModel.Password)).Returns(true);

            var result = await _accountService.CreateAccount(createModel);

            Assert.Equal(account.Email, result.Email);
            Assert.Null(result.Password);
            Assert.Null(result.Salt);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task LoginSuccess()
        {
            //Arrange
            var id = Guid.NewGuid();
            const string email = "iemand@gmail.com";
            const string password = "MyV3rysecurepw!!2";
            var encryptedPassword = Encoding.ASCII.GetBytes(password);
            var salt = new byte[] {0x20, 0x20, 0x20, 0x20};
            const string token = "zqawsexdctvbyunimo";

            var account = new Account
            {
                Id = id,
                Email = email,
                Password = encryptedPassword,
                Salt = salt
            };

            var login = new LoginModel
            {
                Email = account.Email,
                Password = password
            };
            _repository.Setup(x => x.Get(email)).ReturnsAsync(account);
            _hasher.Setup(x => x.VerifyHash(password, salt, encryptedPassword)).ReturnsAsync(true);
            _jwtGenerator.Setup(x => x.GenerateJWT(account.Id)).Returns(token);

            //Act
            var result = await _accountService.Login(login);


            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, account.Id);
        }

        [Fact]
        public async Task LoginInvalid()
        {
            //Arrange
            var id = Guid.NewGuid();
            const string email = "iemand@gmail.com";
            const string password = "MyV3rysecurepw!!2";
            var encryptedPassword = Encoding.ASCII.GetBytes(password);
            var salt = new byte[] {0x20, 0x20, 0x20, 0x20};
            const string token = "zqawsexdctvbyunimo";

            var account = new Account
            {
                Id = id,
                Email = email,
                Password = encryptedPassword,
                Salt = salt
            };

            var login = new LoginModel
            {
                Email = account.Email,
                Password = password
            };
            _repository.Setup(x => x.Get(email)).ReturnsAsync(account);
            _hasher.Setup(x => x.VerifyHash("NotMy!Password1", salt, encryptedPassword)).ReturnsAsync(true);
            _jwtGenerator.Setup(x => x.GenerateJWT(account.Id)).Returns(token);

            //Act
            var result = await Assert.ThrowsAsync<ArgumentException>(() => _accountService.Login(login));
            
            //Assert
            Assert.NotNull(result);
            Assert.IsType<ArgumentException>(result);
        }

        [Fact]
        public async Task UpdatePasswordSuccess()
        {
            var id = Guid.NewGuid();
            const string email = "iemand@gmail.com";
            const string password = "MyV3rysecurepw!!2";
            var encryptedPassword = Encoding.ASCII.GetBytes(password);
            var salt = new byte[] {0x20, 0x20, 0x20, 0x20};
            const string token = "zqawsexdctvbyunimo";
            
            var account = new Account
            {
                Id = id,
                Email = email,
                Password = encryptedPassword,
                Salt = salt
            };
            
            var updatedAccount = new Account
            {
                Id = id,
                Email = email,
                Password = encryptedPassword,
                Salt = salt
            };
            
            var passwordModel = new UpdatePasswordModel()
            {
                OldPassword =  password,
                NewPassword = "Myn3westV3ryS3cur3password12345!@"
            };
            
            _repository.Setup(x => x.Get(id)).ReturnsAsync(account);
            _repository.Setup(x => x.Update(account.Id, account)).ReturnsAsync(updatedAccount);
            _hasher.Setup(x => x.VerifyHash(password, salt, encryptedPassword)).ReturnsAsync(true);
            _jwtGenerator.Setup(x => x.GenerateJWT(account.Id)).Returns(token);

            var result = await _accountService.UpdatePassword(account.Id, passwordModel);

            Assert.NotNull(result);            
        }

        [Fact]
        public async Task UpdatePasswordWrongPassword()
        {
            var id = Guid.NewGuid();
            const string email = "iemand@gmail.com";
            const string password = "MyV3rysecurepw!!2";
            var encryptedPassword = Encoding.ASCII.GetBytes(password);
            var salt = new byte[] {0x20, 0x20, 0x20, 0x20};
            const string token = "zqawsexdctvbyunimo";
            
            var account = new Account
            {
                Id = id,
                Email = email,
                Password = encryptedPassword,
                Salt = salt
            };
            
            
            var updatedAccount = new Account
            {
                Id = id,
                Email = email,
                Password = encryptedPassword,
                Salt = salt
            };
            
            var passwordModel = new UpdatePasswordModel()
            {
                OldPassword =  "NietHetGoedeW4chtwoord12!",
                NewPassword = "Myn3westV3ryS3cur3password12345!@"
            };
            
            
            _repository.Setup(x => x.Get(id)).ReturnsAsync(account);
            _repository.Setup(x => x.Update(account.Id, account)).ReturnsAsync(updatedAccount);
            _hasher.Setup(x => x.VerifyHash(password, salt, encryptedPassword)).ReturnsAsync(true);
            _jwtGenerator.Setup(x => x.GenerateJWT(account.Id)).Returns(token);

            var result =
                await Assert.ThrowsAsync<ArgumentException>(() =>
                    _accountService.UpdatePassword(account.Id, passwordModel));

            Assert.NotNull(result);
            Assert.IsType<ArgumentException>(result);

        }

        [Fact]
        public async Task UpdateAccountSuccess()
        {
            var id = Guid.NewGuid();
            const string emailOld = "iemand@gmail.com";
            const string emailNew = "mijn@nieuwe.email";
            
            var account = new Account
            {
                Id = id,
                Email = emailOld
            };

            var update = new UpdateAccountModel
            {
                Email = emailNew,
            };
            
            var updatedAccount = new Account
            {
                Id = account.Id,
                Email = update.Email
            };

            _repository.Setup(x => x.Get(id)).ReturnsAsync(account);
            _repository.Setup(x => x.Update(account.Id, account)).ReturnsAsync(updatedAccount);
            

            var result = await _accountService.UpdateAccount(id, update);
            
            Assert.NotEqual(emailOld, result.Email);
        }
        
        [Fact]
        public async Task GetAccountSuccess()
        {
            var id = Guid.NewGuid();
            const string email = "iemand@gmail.com";
            const string password = "MyV3rysecurepw!!2";
            var encryptedPassword = Encoding.ASCII.GetBytes(password);
            var salt = new byte[] {0x20, 0x20, 0x20, 0x20};

            var account = new Account
            {
                Id = id,
                Email = email,
                Password = encryptedPassword,
                Salt = salt
            };
            
            _repository.Setup(x => x.Get(id)).ReturnsAsync(account);

            var result = await _accountService.GetAccount(account.Id);
            
            Assert.Equal(id, result.Id);
            Assert.Equal(account.Email, result.Email);
            Assert.Null(result.Password);
            Assert.Null(result.Salt);
        }
        
        [Fact]
        public async Task GetAccountInvalidId()
        {
            var id = Guid.NewGuid();
            const string email = "iemand@gmail.com";
            const string password = "MyV3rysecurepw!!2";
            var encryptedPassword = Encoding.ASCII.GetBytes(password);
            var salt = new byte[] {0x20, 0x20, 0x20, 0x20};

            var account = new Account
            {
                Id = id,
                Email = email,
                Password = encryptedPassword,
                Salt = salt
            };
            
            _repository.Setup(x => x.Get(id)).ReturnsAsync(account);

            var result =
                await Assert.ThrowsAsync<NullReferenceException>(() =>
                    _accountService.GetAccount(Guid.Empty));

            Assert.NotNull(result);
            Assert.IsType<NullReferenceException>(result);
        }
    }
}