using Banking.Shared;
using Microsoft.EntityFrameworkCore;

namespace Banking.AccountApi.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="db">The database context for accessing account data.</param>
        public AccountRepository(BankingContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Creates a new account in the database.
        /// </summary>
        /// <param name="account">The account entity to be created.</param>
        /// <returns>
        /// The created <see cref="Account"/> object.
        /// </returns>
        public async Task<Account> CreateAccountAsync(Account account)
        {
            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();
            return account;
        }

        /// <summary>
        /// Retrieves an account from the database by account number.
        /// </summary>
        /// <param name="accountNumber">The account number to search for.</param>
        /// <returns>
        /// The <see cref="Account"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<Account?> GetAccountAsync(int accountNumber)
        {
            return await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        /// <summary>
        /// Retrieves all accounts from the database.
        /// </summary>
        /// <returns>
        /// A collection of all <see cref="Account"/> entities.
        /// </returns>
        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await db.Accounts.ToListAsync();
        }
    }
}
