using Banking.AccountApi.DTOs;
using Banking.AccountApi.Repositories;
using Banking.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Banking.AccountApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountAPIController : ControllerBase
    {
        private readonly IAccountRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountAPIController"/> class.
        /// </summary>
        /// <param name="repository">The repository for managing account data.</param>
        public AccountAPIController(IAccountRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Retrieves all accounts from the system.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{Account}"/> containing all accounts.
        /// Returns an empty collection if no accounts exist.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Account>))]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            IEnumerable<Account> accounts = await repository.GetAllAccountsAsync();
            return Ok(accounts);
        }

        /// <summary>
        /// Retrieves a specific account by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number to look up.</param>
        /// <returns>
        /// An <see cref="Account"/> object if found; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{accountNumber}", Name = nameof(GetAccount))]
        [ProducesResponseType(200, Type = typeof(Account))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAccount(int accountNumber)
        {
            Account? account = await repository.GetAccountAsync(accountNumber);
            if (account is null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        /// <summary>
        /// Creates a new account in the system.
        /// </summary>
        /// <param name="accountDTO">
        /// A <see cref="AccountDTO"/> object containing the account details to create.
        /// </param>
        /// <returns>
        /// A 201 Created response with the created <see cref="Account"/> if successful.
        /// Returns a 400 Bad Request response for invalid input.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Account))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAccount([FromBody] AccountDTO accountDTO)
        {
            if (accountDTO is null)
            {
                return BadRequest();
            }
            if (accountDTO.initialBalance < 0)
            {
                return BadRequest("Initial balance is less than 0");
            }
            Account account = new() { Name = accountDTO.Name, Balance = accountDTO.initialBalance };

            Account? addedAccount = await repository.CreateAccountAsync(account);
            if (addedAccount is null)
            {
                return BadRequest("Repository failed to create account");
            }
            return CreatedAtRoute(
                routeName: nameof(GetAccount),
                routeValues: new { accountNumber = addedAccount.AccountNumber },
                value: addedAccount
            );
        }
    }
}
