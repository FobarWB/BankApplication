namespace Banking.TransactionApi.DTOs
{
    public class TransferTransactionDTO
    {
        public int FromAccountNumber { get; set; }
        public int ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}