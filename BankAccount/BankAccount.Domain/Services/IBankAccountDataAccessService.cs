
using BankAccount.Domain.Entities.BankAccount;
using BankAccount.Domain.models;

namespace BankAccount.Domain.Services
{
    public interface IBankAccountDataAccessService
    {
        void Create(DbAccount account);
        List<DbAccount> GetAllAccount();
        DbAccount GetAccount(Guid reference);
        float GetBalance(Guid reference);
        List<DbTransaction> GetTransactions(Guid reference);
        bool DebitAccount(Guid reference, float ammount, string label);
        bool CreditAccount(Guid reference, float ammount, string label);
    }
}
