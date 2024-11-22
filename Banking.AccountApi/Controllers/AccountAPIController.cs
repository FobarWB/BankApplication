using Banking.Shared;
using Banking.AccountApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Banking.AccountApi.DTOs;

namespace Banking.AccountApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountAPIController: ControllerBase
    {
        private readonly IAccountRepository repository;

        public AccountAPIController(IAccountRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/accounts
        // this will always return a list of accounts (but it might be empty)
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Account>))]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            IEnumerable<Account> accounts = await repository.GetAllAccountsAsync();
            return Ok(accounts);
        }

        // GET: api/accounts/[accountNumber]
        // this can return account if number found or empty otherwise
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

        //POST: api/accounts
        //BODY: Account (JSON, XML)
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
            Account account = new()
            {
                Name = accountDTO.Name,
                Balance = accountDTO.initialBalance,
            };

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