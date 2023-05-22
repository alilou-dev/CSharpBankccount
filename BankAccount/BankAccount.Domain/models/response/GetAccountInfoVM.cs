
using BankAccount.Domain.Entities.BankAccount;

namespace BankAccount.Domain.models.response
{
    public class GetAccountInfoVM
    {
        public string? AccountRef { get; set; }
        public string? AccountLabel { get; set; }
        public float? Balance { get; set; }

        public GetAccountInfoVM(string accountRef, string accountLabel, float balance)
        {
            AccountLabel = accountRef;
            Balance = balance;
            AccountLabel = accountLabel;
        }

        public GetAccountInfoVM(DbAccount dbAccount)
        {
            AccountRef = dbAccount.Reference.ToString();
            AccountLabel = dbAccount.Label; 
            Balance = dbAccount.Balance;
        }
    }
}
