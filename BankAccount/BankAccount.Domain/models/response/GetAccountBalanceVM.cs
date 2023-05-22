namespace BankAccount.Domain.models.response
{
    public class GetAccountBalanceVM
    {
        public float? Blance { get; set; }

        public GetAccountBalanceVM(float blance)
        {
            Blance = blance;
        }
    }
}
