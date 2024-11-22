using Banking.Shared;

namespace Banking.TransactionAPI.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction?> DepositAsync(Transaction transaction);
        Task<Transaction?> WithdrawAsync(Transaction transaction);
        Task<Transaction?> TransferAsync(Transaction transaction);
        Task<Transaction?> GetTransactionAsync(int transactionId);
    }
}