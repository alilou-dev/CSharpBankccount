
namespace BankAccount.Domain.models.response
{
    public class GetAccountBalanceResponseVM
    {
        public float Balance { get; set; }
        public GetAccountBalanceResponseVM(float balance)
        {
            Balance = balance;
        }
    }
}
