using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Banking.Shared;
using Microsoft.EntityFrameworkCore;

namespace Banking.AccountApi.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingContext db;
        
        public AccountRepository(BankingContext db)
        {
            this.db = db;
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> GetAccountAsync(int accountNumber)
        {
            return await db.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await db.Accounts.ToListAsync();
        }
    }
}