using System.ComponentModel.DataAnnotations;

namespace UPITransaction.DataAccessLayer.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsUpiEnabled { get; set; } = false;

        public decimal Balance { get; set; } = 0;

        public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
    }
}
