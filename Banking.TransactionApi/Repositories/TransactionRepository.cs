using Banking.Shared;
using Microsoft.EntityFrameworkCore;

namespace Banking.TransactionAPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
        /// </summary>
        /// <param name="db">The database context for managing transactions and accounts.</param>
        public TransactionRepository(BankingContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Processes a deposit transaction by adding the specified amount to the target account.
        /// </summary>
        /// <param name="transaction">The transaction details, including the target account and amount.</param>
        /// <returns>
        /// The completed <see cref="Transaction"/> object if successful;
        /// otherwise, <c>null</c> if the account does not exist.
        /// </returns>
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

        /// <summary>
        /// Processes a withdrawal transaction by deducting the specified amount from the source account.
        /// </summary>
        /// <param name="transaction">The transaction details, including the source account and amount.</param>
        /// <returns>
        /// The completed <see cref="Transaction"/> object if successful;
        /// otherwise, <c>null</c> if the account does not exist or the balance is insufficient.
        /// </returns>
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

        /// <summary>
        /// Processes a transfer transaction between two accounts.
        /// </summary>
        /// <param name="transaction">The transaction details, including source, target accounts, and amount.</param>
        /// <returns>
        /// The completed <see cref="Transaction"/> object if successful;
        /// otherwise, <c>null</c> if any account does not exist or the source account has insufficient balance.
        /// </returns>
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

        /// <summary>
        /// Retrieves a transaction by its unique identifier.
        /// </summary>
        /// <param name="transactionId">The unique identifier of the transaction.</param>
        /// <returns>
        /// The <see cref="Transaction"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<Transaction?> GetTransactionAsync(int transactionId)
        {
            return await db.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }
    }
}
