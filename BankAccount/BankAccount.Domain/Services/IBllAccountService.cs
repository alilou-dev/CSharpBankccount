
using BankAccount.Domain.models;
using BankAccount.Domain.models.request;

namespace BankAccount.Domain.Services
{
    public interface IBllAccountService
    {
        Response CreateAccount(string label);
        Response GetAllAccount();
        Response GetAccountInfo(Guid reference);
        Response GetBalanceAccount(Guid reference);
        Response DebitOrCreditAccount(CreditOrDebitAccountVM model, Guid accountRef);
        Response GetAllTransactions(Guid accountRef);
    }
}
