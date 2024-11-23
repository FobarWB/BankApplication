using Banking.AccountApi.Controllers;
using Banking.AccountApi.DTOs;
using Banking.AccountApi.Repositories;
using Banking.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Banking.XUnitTests
{
    public class AccountAPIControllerTests
    {
        private readonly Mock<IAccountRepository> _mockRepository;
        private readonly AccountAPIController _controller;
        private readonly List<Account> _fakeAccounts;

        public AccountAPIControllerTests()
        {
            _fakeAccounts =
            [
                new Account
                {
                    AccountNumber = 1,
                    Name = "John Doe",
                    Balance = 1000,
                },
                new Account
                {
                    AccountNumber = 2,
                    Name = "Jane Doe",
                    Balance = 2000,
                },
                new Account
                {
                    AccountNumber = 3,
                    Name = "Alice Brown",
                    Balance = 3000,
                },
            ];

            _mockRepository = new();
            _controller = new(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAccounts_ReturnsAllAccounts()
        {
            _mockRepository.Setup(repo => repo.GetAllAccountsAsync()).ReturnsAsync(_fakeAccounts);

            var result = await _controller.GetAccounts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var accounts = Assert.IsAssignableFrom<IEnumerable<Account>>(okResult.Value);
            Assert.Equal(_fakeAccounts, accounts);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetAccount_ReturnsAccount_WhenAccountExists(int accountNumber)
        {
            var expectedAccount = _fakeAccounts.First(a => a.AccountNumber == accountNumber);
            _mockRepository
                .Setup(repo => repo.GetAccountAsync(accountNumber))
                .ReturnsAsync(expectedAccount);

            var result = await _controller.GetAccount(accountNumber);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var account = Assert.IsType<Account>(okResult.Value);
            Assert.Equal(expectedAccount, account);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(99)]
        public async Task GetAccount_ReturnsNotFound_WhenAccountDoesNotExist(int accountNumber)
        {
            _mockRepository
                .Setup(repo => repo.GetAccountAsync(accountNumber))
                .ReturnsAsync((Account)null);

            var result = await _controller.GetAccount(accountNumber);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateAccount_ReturnsCreatedAccount()
        {
            var accountDTO = new AccountDTO { Name = "New Account", initialBalance = 1500 };
            var expectedAccount = new Account
            {
                AccountNumber = 4,
                Name = "New Account",
                Balance = 1500,
            };

            _mockRepository
                .Setup(repo => repo.CreateAccountAsync(It.IsAny<Account>()))
                .ReturnsAsync(expectedAccount);

            var result = await _controller.CreateAccount(accountDTO);

            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnedAccount = Assert.IsType<Account>(createdAtRouteResult.Value);

            Assert.Equal(expectedAccount, returnedAccount);
        }

        [Fact]
        public async Task CreateAccount_ReturnsBadRequest_WhenDTOIsNull()
        {
            var result = await _controller.CreateAccount(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CreateAccount_ReturnsBadRequest_WhenInitialBalanceIsNegative()
        {
            var accountDTO = new AccountDTO { Name = "Invalid Account", initialBalance = -500 };

            var result = await _controller.CreateAccount(accountDTO);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
