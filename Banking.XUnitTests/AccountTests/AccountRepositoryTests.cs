using Banking.AccountApi.Repositories;
using Banking.Shared;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

namespace Banking.XUnitTests;

public class AccountRepositoryTests
{
    private Mock<DbSet<Account>> _mockSet;
    private Mock<BankingContext> _mockContext;
    private AccountRepository _repository;
    private List<Account> fakeAccounts;

    public AccountRepositoryTests()
    {
        fakeAccounts =
        [
            new Account
            {
                AccountNumber = 0,
                Name = "John Doe",
                Balance = 1750,
            },
            new Account
            {
                AccountNumber = 1,
                Name = "Alice Brown",
                Balance = 1500,
            },
            new Account
            {
                AccountNumber = 2,
                Name = "Charlie Davis",
                Balance = 2400,
            },
            new Account
            {
                AccountNumber = 3,
                Name = "Diana Wilson",
                Balance = 3600,
            },
            new Account
            {
                AccountNumber = 4,
                Name = "Frank Johnson",
                Balance = 2900,
            },
            new Account
            {
                AccountNumber = 5,
                Name = "Eve Taylor",
                Balance = 3050,
            },
            new Account
            {
                AccountNumber = 6,
                Name = "Bob Smith",
                Balance = 1300,
            },
            new Account
            {
                AccountNumber = 7,
                Name = "Grace Anderson",
                Balance = 4200,
            },
            new Account
            {
                AccountNumber = 8,
                Name = "Henry Thomas",
                Balance = 2700,
            },
            new Account
            {
                AccountNumber = 9,
                Name = "Jane Miller",
                Balance = 500,
            },
            new Account
            {
                AccountNumber = 10,
                Name = "Oliver Davis",
                Balance = 1850,
            },
            new Account
            {
                AccountNumber = 11,
                Name = "Sophia Brown",
                Balance = 2100,
            },
            new Account
            {
                AccountNumber = 12,
                Name = "Liam Wilson",
                Balance = 1950,
            },
            new Account
            {
                AccountNumber = 13,
                Name = "Emma Johnson",
                Balance = 3100,
            },
            new Account
            {
                AccountNumber = 14,
                Name = "Lucas Taylor",
                Balance = 3450,
            },
            new Account
            {
                AccountNumber = 15,
                Name = "Mia Anderson",
                Balance = 2750,
            },
            new Account
            {
                AccountNumber = 16,
                Name = "Noah Thomas",
                Balance = 2300,
            },
            new Account
            {
                AccountNumber = 17,
                Name = "Olivia Miller",
                Balance = 1650,
            },
            new Account
            {
                AccountNumber = 18,
                Name = "Ethan Doe",
                Balance = 3700,
            },
            new Account
            {
                AccountNumber = 19,
                Name = "Isabella Smith",
                Balance = 2900,
            },
        ];

        _mockSet = fakeAccounts.BuildMock().BuildMockDbSet();
        _mockContext = new();
        _mockContext.Setup(m => m.Accounts).Returns(_mockSet.Object);
        _repository = new(_mockContext.Object);
    }

    [Fact]
    public async Task CreateAccountAsync_AddsAccountToDatabase()
    {
        Account account = new() { Name = "John Doe", Balance = 1000 };

        await _repository.CreateAccountAsync(account);

        _mockSet.Verify(m => m.AddAsync(account, default), Times.Once);
        _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAccountAsync_ReturnsCreatedAccount()
    {
        Account account = new() { Name = "John Doe", Balance = 1000 };

        Account result = await _repository.CreateAccountAsync(account);

        Assert.Equal(account, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public async Task GetAccountAsync_ReturnsAccount_WhenAccountExists(int accountNumber)
    {
        Account expected = fakeAccounts[accountNumber];

        Account result = await _repository.GetAccountAsync(accountNumber);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(20)]
    public async Task GetAccountAsync_ReturnsNull_WhenAccountDoesNotExist(int accountNumber)
    {
        Account result = await _repository.GetAccountAsync(accountNumber);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAccountsAsync_ReturnsAllAccounts()
    {
        IEnumerable<Account> expected = fakeAccounts.AsEnumerable();

        IEnumerable<Account> result = await _repository.GetAllAccountsAsync();

        Assert.Equal(expected, result);
    }
}
