namespace Banking.AccountApi.DTOs
{
    public class AccountDTO
    {
        public string Name { get; set; } = null!;
        public decimal initialBalance { get; set; }
    }
}