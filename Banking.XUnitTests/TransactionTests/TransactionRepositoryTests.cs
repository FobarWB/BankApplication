using Banking.Shared;
using Banking.TransactionAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

namespace Banking.XUnitTests
{
    public class TransactionRepositoryTests
    {
        private readonly Mock<DbSet<Account>> _mockAccountSet;
        private readonly Mock<DbSet<Transaction>> _mockTransactionSet;
        private readonly Mock<BankingContext> _mockContext;
        private readonly TransactionRepository _repository;
        private readonly List<Account> _fakeAccounts;
        private readonly List<Transaction> _fakeTransactions;

        public TransactionRepositoryTests()
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
                    Name = "Jane Smith",
                    Balance = 2000,
                },
            ];

            _fakeTransactions =
            [
                new Transaction
                {
                    TransactionId = 1,
                    ToAccountNumber = 1,
                    Amount = 500,
                    Type = TransactionType.Deposit,
                },
                new Transaction
                {
                    TransactionId = 2,
                    FromAccountNumber = 2,
                    Amount = 600,
                    Type = TransactionType.Withdraw,
                },
                new Transaction
                {
                    TransactionId = 3,
                    FromAccountNumber = 2,
                    ToAccountNumber = 1,
                    Amount = 300,
                    Type = TransactionType.Transfer,
                },
            ];

            _mockAccountSet = _fakeAccounts.BuildMock().BuildMockDbSet();
            _mockTransactionSet = _fakeTransactions.BuildMock().BuildMockDbSet();

            _mockContext = new Mock<BankingContext>();
            _mockContext.Setup(c => c.Accounts).Returns(_mockAccountSet.Object);
            _mockContext.Setup(c => c.Transactions).Returns(_mockTransactionSet.Object);

            _repository = new TransactionRepository(_mockContext.Object);
        }

        [Fact]
        public async Task DepositAsync_AddsTransactionToDatabase()
        {
            var transaction = new Transaction
            {
                ToAccountNumber = 1,
                Amount = 500,
                Type = TransactionType.Deposit,
            };

            var result = await _repository.DepositAsync(transaction);

            _mockTransactionSet.Verify(m => m.AddAsync(transaction, default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DepositAsync_AddsFundsToAccount()
        {
            var transaction = new Transaction
            {
                ToAccountNumber = 1,
                Amount = 500,
                Type = TransactionType.Deposit,
            };

            var result = await _repository.DepositAsync(transaction);

            Assert.NotNull(result);
            Assert.Equal(1500, _fakeAccounts[0].Balance);
        }

        [Fact]
        public async Task DepositAsync_ReturnsCreatedTransaction()
        {
            var transaction = new Transaction
            {
                ToAccountNumber = 1,
                Amount = 500,
                Type = TransactionType.Deposit,
            };

            var result = await _repository.DepositAsync(transaction);

            Assert.Equal(transaction, result);
        }

        [Fact]
        public async Task DepositAsync_ReturnsNull_WhenAccountNotFound()
        {
            var transaction = new Transaction
            {
                ToAccountNumber = 99,
                Amount = 500,
                Type = TransactionType.Deposit,
            };

            var result = await _repository.DepositAsync(transaction);

            Assert.Null(result);
        }

        [Fact]
        public async Task WithdrawAsync_AddsTransactionToDatabase()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                Amount = 500,
                Type = TransactionType.Withdraw,
            };

            var result = await _repository.WithdrawAsync(transaction);

            _mockTransactionSet.Verify(m => m.AddAsync(transaction, default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task WithdrawAsync_RemovesFundsFromAccount()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                Amount = 500,
                Type = TransactionType.Withdraw,
            };

            var result = await _repository.WithdrawAsync(transaction);

            Assert.NotNull(result);
            Assert.Equal(500, _fakeAccounts[0].Balance);
        }

        [Fact]
        public async Task WithdrawAsync_ReturnsCreatedTransaction()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                Amount = 500,
                Type = TransactionType.Withdraw,
            };

            var result = await _repository.WithdrawAsync(transaction);

            Assert.Equal(transaction, result);
        }

        [Fact]
        public async Task WithdrawAsync_ReturnsNull_WhenAccountNotFound()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 99,
                Amount = 500,
                Type = TransactionType.Withdraw,
            };

            var result = await _repository.WithdrawAsync(transaction);

            Assert.Null(result);
        }

        [Fact]
        public async Task WithdrawAsync_ReturnsNull_WhenAmountIsGreaterThanBalance()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                Amount = 10000,
                Type = TransactionType.Withdraw,
            };

            var result = await _repository.WithdrawAsync(transaction);

            Assert.Null(result);
        }

        [Fact]
        public async Task TransferAsync_AddsTransactionToDatabase()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                ToAccountNumber = 2,
                Amount = 500,
                Type = TransactionType.Transfer,
            };

            await _repository.TransferAsync(transaction);

            _mockTransactionSet.Verify(m => m.AddAsync(transaction, default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task TransferAsync_RemovesFundsFromAccount()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                ToAccountNumber = 2,
                Amount = 500,
                Type = TransactionType.Transfer,
            };

            await _repository.TransferAsync(transaction);

            Assert.Equal(500, _fakeAccounts.First(a => a.AccountNumber == 1).Balance);
        }

        [Fact]
        public async Task TransferAsync_AddsFundsToAccount()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                ToAccountNumber = 2,
                Amount = 500,
                Type = TransactionType.Transfer,
            };

            await _repository.TransferAsync(transaction);

            Assert.Equal(2500, _fakeAccounts.First(a => a.AccountNumber == 2).Balance);
        }

        [Fact]
        public async Task TransferAsync_ReturnsCreatedTransaction()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                ToAccountNumber = 2,
                Amount = 500,
                Type = TransactionType.Transfer,
            };

            var result = await _repository.TransferAsync(transaction);

            Assert.Equal(transaction, result);
        }

        [Fact]
        public async Task TransferAsync_ReturnsNull_WhenFromAccountNotFound()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 99,
                ToAccountNumber = 2,
                Amount = 500,
                Type = TransactionType.Transfer,
            };

            var result = await _repository.TransferAsync(transaction);

            Assert.Null(result);
        }

        [Fact]
        public async Task TransferAsync_ReturnsNull_WhenToAccountNotFound()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                ToAccountNumber = 99,
                Amount = 500,
                Type = TransactionType.Transfer,
            };

            var result = await _repository.TransferAsync(transaction);

            Assert.Null(result);
        }

        [Fact]
        public async Task TransferAsync_ReturnsNull_WhenAmountIsGreaterThanBalance()
        {
            var transaction = new Transaction
            {
                FromAccountNumber = 1,
                ToAccountNumber = 2,
                Amount = 10000,
                Type = TransactionType.Transfer,
            };

            var result = await _repository.TransferAsync(transaction);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetTransactionAsync_ReturnsTransaction_WhenTransactionExists(
            int transactionId
        )
        {
            Transaction expected = _fakeTransactions.First(t => t.TransactionId == transactionId);

            var result = await _repository.GetTransactionAsync(transactionId);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(99)]
        public async Task GetTransactionAsync_ReturnsNull_WhenTransactionNotFound(int transactionId)
        {
            var result = await _repository.GetTransactionAsync(transactionId);

            Assert.Null(result);
        }
    }
}
