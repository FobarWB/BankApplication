using Banking.Shared;
using Banking.TransactionAPI.Controllers;
using Banking.TransactionApi.DTOs;
using Banking.TransactionAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Banking.XUnitTests.TransactionTests
{
    public class TransactionAPIControllerTests
    {
        private readonly Mock<ITransactionRepository> _mockRepository;
        private readonly TransactionAPIController _controller;
        private readonly List<Transaction> _fakeTransactions;

        public TransactionAPIControllerTests()
        {
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
            _mockRepository = new Mock<ITransactionRepository>();
            _controller = new TransactionAPIController(_mockRepository.Object);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetTransaction_ReturnsTransaction_WhenTransactionExists(int transactionId)
        {
            var expectedTransaction = _fakeTransactions.First(t =>
                t.TransactionId == transactionId
            );
            _mockRepository
                .Setup(repo => repo.GetTransactionAsync(transactionId))
                .ReturnsAsync(expectedTransaction);

            var result = await _controller.GetTransaction(transactionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var transaction = Assert.IsType<Transaction>(okResult.Value);
            Assert.Equal(expectedTransaction, transaction);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(99)]
        public async Task GetTransaction_ReturnsNotFound_WhenTransactionNotExist(int transactionId)
        {
            _mockRepository
                .Setup(repo => repo.GetTransactionAsync(transactionId))
                .ReturnsAsync((Transaction)null);

            var result = await _controller.GetTransaction(transactionId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Deposit_ReturnsCreatedTransaction()
        {
            var transactionDTO = new SelfTransactionDTO { AccountNumber = 1, Amount = 500 };
            var expected = new Transaction
            {
                TransactionId = 1,
                ToAccountNumber = transactionDTO.AccountNumber,
                Amount = transactionDTO.Amount,
                Type = TransactionType.Deposit,
            };

            _mockRepository
                .Setup(repo => repo.DepositAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(expected);

            var result = await _controller.Deposite(transactionDTO);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<Transaction>(createdAtActionResult.Value);
            Assert.Equal(expected, returned);
        }

        [Fact]
        public async Task Deposit_ReturnsBadRequest_WhenDTOIsNull()
        {
            var result = await _controller.Deposite(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Deposit_ReturnsBadRequest_WhenRepositoryFailed()
        {
            var transactionDTO = new SelfTransactionDTO { AccountNumber = 1, Amount = 500 };

            _mockRepository
                .Setup(repo => repo.DepositAsync(It.IsAny<Transaction>()))
                .ReturnsAsync((Transaction)null);

            var result = await _controller.Deposite(transactionDTO);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Withdraw_ReturnsCreatedTransaction()
        {
            var transactionDTO = new SelfTransactionDTO { AccountNumber = 1, Amount = 500 };
            var expected = new Transaction
            {
                TransactionId = 1,
                FromAccountNumber = transactionDTO.AccountNumber,
                Amount = transactionDTO.Amount,
                Type = TransactionType.Withdraw,
            };

            _mockRepository
                .Setup(repo => repo.WithdrawAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(expected);

            var result = await _controller.Withdraw(transactionDTO);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<Transaction>(createdAtActionResult.Value);
            Assert.Equal(expected, returned);
        }

        [Fact]
        public async Task Withdraw_ReturnsBadRequest_WhenDTOIsNull()
        {
            var result = await _controller.Withdraw(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Withdraw_ReturnsBadRequest_WhenRepositoryFailed()
        {
            var transactionDTO = new SelfTransactionDTO { AccountNumber = 1, Amount = 500 };

            _mockRepository
                .Setup(repo => repo.WithdrawAsync(It.IsAny<Transaction>()))
                .ReturnsAsync((Transaction)null);

            var result = await _controller.Withdraw(transactionDTO);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Transfer_ReturnsCreatedTransaction()
        {
            var transactionDTO = new TransferTransactionDTO
            {
                FromAccountNumber = 1,
                ToAccountNumber = 2,
                Amount = 500,
            };

            var expected = new Transaction
            {
                TransactionId = 1,
                ToAccountNumber = transactionDTO.ToAccountNumber,
                FromAccountNumber = transactionDTO.FromAccountNumber,
                Amount = transactionDTO.Amount,
                Type = TransactionType.Transfer,
            };

            _mockRepository
                .Setup(repo => repo.TransferAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(expected);

            var result = await _controller.Transfer(transactionDTO);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<Transaction>(createdAtActionResult.Value);
            Assert.Equal(expected, returned);
        }

        [Fact]
        public async Task Transfer_ReturnsBadRequest_WhenDTOIsNull()
        {
            var result = await _controller.Transfer(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Transfer_ReturnsBadRequest_WhenRepositoryFailed()
        {
            var transactionDTO = new TransferTransactionDTO
            {
                FromAccountNumber = 1,
                ToAccountNumber = 2,
                Amount = 500,
            };

            _mockRepository
                .Setup(repo => repo.TransferAsync(It.IsAny<Transaction>()))
                .ReturnsAsync((Transaction)null);

            var result = await _controller.Transfer(transactionDTO);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
