
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccount.Domain.Entities.BankAccount
{
    public class DbAccount
    {
        public int Id { get; set; }
        public Guid Reference { get; set; }
        public virtual List<DbTransaction> Transactions { get; set; }
        public float Balance { get; set; }
        public string Label { get; set; }

    }
}
