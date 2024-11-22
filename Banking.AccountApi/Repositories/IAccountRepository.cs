using Banking.Shared;

namespace Banking.AccountApi.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> CreateAccountAsync(Account account);
        Task<Account?> GetAccountAsync(int accountNumber);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
    }
}