
using BankAccount.Domain.Entities.BankAccount;
using BankAccount.Domain.models.request;

namespace BankAccount.Domain.models.response
{
    public class GetAllTransactionsVM
    {
        public string TransactionType { get; set; }
        public float Amount { get; set; }
        public Guid AccountReference { get; set; }
        public string Label { get; set; }

        public GetAllTransactionsVM(DbTransaction transaction)
        {
            TransactionType = transaction.Type.ToString();
            Amount = transaction.Amount;
            Label = transaction.Label;
            AccountReference = transaction.Account.Reference;
        }
    }
}
