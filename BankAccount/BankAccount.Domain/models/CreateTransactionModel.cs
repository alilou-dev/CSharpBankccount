
using BankAccount.Domain.models.request;

namespace BankAccount.Domain.models
{
    public class CreateTransactionModel
    {
        public Guid AccountReference { get; set; }
        public float Ammount { get; set; }
        public TransactionType TransactionType { get; set; }
        public CreateTransactionModel(CreditOrDebitAccountVM vm, Guid accountRef)
        {
            AccountReference = accountRef;
            Ammount = vm.Ammount;
            TransactionType = vm.TransactionType.ToLower() == "d" ? TransactionType.Debit : TransactionType.Credit;
        }
    }
}
