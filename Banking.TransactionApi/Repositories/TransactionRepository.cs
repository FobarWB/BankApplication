using Banking.Shared;
using Microsoft.EntityFrameworkCore;

namespace Banking.TransactionAPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingContext db;

        public TransactionRepository(BankingContext db)
        {
            this.db = db;
        }

        public async Task<Transaction?> DepositAsync(Transaction transaction)
        {
            Account? account = await db.Accounts.FirstOrDefaultAsync(a =>
                a.AccountNumber == transaction.ToAccountNumber
            );
            if (account is null)
            {
                return null;
            }
            account.Balance += transaction.Amount;
            db.Accounts.Update(account);
            await db.Transactions.AddAsync(transaction);
            await db.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> WithdrawAsync(Transaction transaction)
        {
            Account? account = await db.Accounts.FirstOrDefaultAsync(a =>
                a.AccountNumber == transaction.FromAccountNumber
            );
            if (account is null)
            {
                return null;
            }
            if (transaction.Amount > account.Balance)
            {
                return null;
            }
            account.Balance -= transaction.Amount;
            db.Accounts.Update(account);
            await db.Transactions.AddAsync(transaction);
            await db.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> TransferAsync(Transaction transaction)
        {
            Account? accountFrom = await db.Accounts.FirstOrDefaultAsync(a =>
                a.AccountNumber == transaction.FromAccountNumber
            );
            if (accountFrom is null)
            {
                return null;
            }
            Account? accountTo = await db.Accounts.FirstOrDefaultAsync(a =>
                a.AccountNumber == transaction.ToAccountNumber
            );
            if (accountTo is null)
            {
                return null;
            }
            if (transaction.Amount > accountFrom.Balance)
            {
                return null;
            }
            accountFrom.Balance -= transaction.Amount;
            accountTo.Balance += transaction.Amount;
            db.Accounts.UpdateRange(accountFrom, accountTo);
            await db.Transactions.AddAsync(transaction);
            await db.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> GetTransactionAsync(int transactionId)
        {
            return await db.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }
    }
}
