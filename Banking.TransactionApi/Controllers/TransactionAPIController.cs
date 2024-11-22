using Banking.Shared;
using Banking.TransactionApi.DTOs;
using Banking.TransactionAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Banking.TransactionAPI.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionAPIController: ControllerBase
    {
        private readonly ITransactionRepository repository;

        public TransactionAPIController(ITransactionRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/transactions/[transactionId]
        // this can return transaction if id found or empty otherwise
        [HttpGet("{transactionId}", Name = nameof(GetTransaction))]
        [ProducesResponseType(200, Type = typeof(Account))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTransaction(int transactionId)
        {
            Transaction? transaction = await repository.GetTransactionAsync(transactionId);
            if (transaction is null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        //POST: api/transactions/deposit
        //BODY: Transaction (JSON, XML)
        [HttpPost("deposit")]
        [ProducesResponseType(201, Type = typeof(Transaction))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Deposite([FromBody] SelfTransactionDTO transactionDTO)
        {
            if (transactionDTO is null)
            {
                return BadRequest();
            }
            Transaction transaction = new()
            {
                ToAccountNumber = transactionDTO.AccountNumber,
                Type = TransactionType.Deposit,
                Amount = transactionDTO.Amount,
            };

            Transaction? addedTransaction = await repository.DepositAsync(transaction);
            if (addedTransaction is null)
            {
                return BadRequest("Repository failed to create transaction");
            }
            return CreatedAtAction(
                actionName: nameof(GetTransaction), 
                routeValues: new { transactionId = addedTransaction.TransactionId },
                value: addedTransaction
            );
        }

        //POST: api/transactions/withdraw
        //BODY: Transaction (JSON, XML)
        [HttpPost("withdraw")]
        [ProducesResponseType(201, Type = typeof(Transaction))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Withdraw([FromBody] SelfTransactionDTO transactionDTO)
        {
            if (transactionDTO is null)
            {
                return BadRequest();
            }
            Transaction transaction = new()
            {
                FromAccountNumber = transactionDTO.AccountNumber,
                Type = TransactionType.Withdraw,
                Amount = transactionDTO.Amount,
            };

            Transaction? addedTransaction = await repository.WithdrawAsync(transaction);
            if (addedTransaction is null)
            {
                return BadRequest("Repository failed to create transaction");
            }
            return CreatedAtAction(
                actionName: nameof(GetTransaction), 
                routeValues: new { transactionId = addedTransaction.TransactionId },
                value: addedTransaction
            );
        }

        //POST: api/transactions/transfer
        //BODY: Transaction (JSON, XML)
        [HttpPost("transfer")]
        [ProducesResponseType(201, Type = typeof(Transaction))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Transfer([FromBody] TransferTransactionDTO transactionDTO)
        {
            if (transactionDTO is null)
            {
                return BadRequest();
            }
            Transaction transaction = new()
            {
                FromAccountNumber = transactionDTO.FromAccountNumber,
                ToAccountNumber = transactionDTO.ToAccountNumber,
                Type = TransactionType.Transfer,
                Amount = transactionDTO.Amount,
            };

            Transaction? addedTransaction = await repository.TransferAsync(transaction);
            if (addedTransaction is null)
            {
                return BadRequest("Repository failed to create transaction");
            }
            return CreatedAtAction(
                actionName: nameof(GetTransaction), 
                routeValues: new { transactionId = addedTransaction.TransactionId },
                value: addedTransaction
            );
        }
    }
}