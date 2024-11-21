using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking.Shared
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int? FromAccountNumber { get; set; } 

        public int? ToAccountNumber { get; set; }

        [Required]
        public TransactionType Type { get; set; } 

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [ForeignKey("FromAccountNumber")]
        public virtual Account? FromAccount { get; set; } 

        [ForeignKey("ToAccountNumber")]
        public virtual Account? ToAccount { get; set; } 
    }
}