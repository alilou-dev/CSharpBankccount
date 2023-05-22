
using System.ComponentModel.DataAnnotations;

namespace BankAccount.Domain.models.request
{
    public class CreditOrDebitAccountVM
    {
        [Required, MaxLength(1)]
        public string TransactionType { get; set; }
        [Required]
        public float Ammount { get; set; }
        public string Label { get; set; }
    }
}
