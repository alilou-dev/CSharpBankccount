using BankAccount.Domain.Entities.Transaction;

namespace BankAccount.Domain.Entities.BankAccount
{
    public class DbTransaction
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; } 
        public float Amount { get; set; }
        public virtual DbAccount Account { get; set; }
        public string Label { get; set; }
    }
}
