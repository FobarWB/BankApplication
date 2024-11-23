using Banking.Shared;
using Banking.TransactionApi.DTOs;
using Banking.TransactionAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Banking.TransactionAPI.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionAPIController : ControllerBase
    {
        private readonly ITransactionRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionAPIController"/> class.
        /// </summary>
        /// <param name="repository">The repository for managing transaction operations.</param>
        public TransactionAPIController(ITransactionRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Retrieves a specific transaction by its ID.
        /// </summary>
        /// <param name="transactionId">The unique identifier of the transaction.</param>
        /// <returns>
        /// A <see cref="Transaction"/> object if found; otherwise, a 404 Not Found response.
        /// </returns>
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

        /// <summary>
        /// Processes a deposit transaction by adding the specified amount to the target account.
        /// </summary>
        /// <param name="transactionDTO">
        /// A <see cref="SelfTransactionDTO"/> containing the account number and the amount to deposit.
        /// </param>
        /// <returns>
        /// A 201 Created response with the created <see cref="Transaction"/> if successful;
        /// otherwise, a 400 Bad Request response for invalid input or processing failure.
        /// </returns>
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

        /// <summary>
        /// Processes a withdrawal transaction by deducting the specified amount from the source account.
        /// </summary>
        /// <param name="transactionDTO">
        /// A <see cref="SelfTransactionDTO"/> containing the account number and the amount to withdraw.
        /// </param>
        /// <returns>
        /// A 201 Created response with the created <see cref="Transaction"/> if successful;
        /// otherwise, a 400 Bad Request response for invalid input or processing failure.
        /// </returns>
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

        /// <summary>
        /// Processes a transfer transaction by moving the specified amount from one account to another.
        /// </summary>
        /// <param name="transactionDTO">
        /// A <see cref="TransferTransactionDTO"/> containing the source account, target account, and the transfer amount.
        /// </param>
        /// <returns>
        /// A 201 Created response with the created <see cref="Transaction"/> if successful;
        /// otherwise, a 400 Bad Request response for invalid input or processing failure.
        /// </returns>
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
