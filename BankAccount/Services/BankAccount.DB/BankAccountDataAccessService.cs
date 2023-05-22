using BankAccount.Domain.Entities.BankAccount;
using BankAccount.Domain.Exceptions;
using BankAccount.Domain.models;
using BankAccount.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace BankAccount.DB
{
    public class BankAccountDataAccessService : IBankAccountDataAccessService
    {
        private readonly BankAccountContext _context;
        public BankAccountDataAccessService(DbContextOptions<BankAccountContext> dbContext)
        {
            _context = new BankAccountContext(dbContext);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public void Create(DbAccount account)
        {
            if (_context.Account.Any(ac => ac.Reference == account.Reference))
            {
                throw new BankAccountException(ErrorCode.AccountAlreadyExist, "Account already exist in db");
            }
            _context.Account.Add(account);
            _context.SaveChanges();
        }

        public List<DbAccount> GetAllAccount()
        {
            return _context.Account.ToList();
        }

        public float GetBalance(Guid reference)
        {
            return _context.Account.Where(ac => ac.Reference == reference).Select(ac => ac.Balance).ToList().First();
        }

        public DbAccount GetAccount(Guid reference)
        {
            return _context.Account.Where(ac => ac.Reference == reference).Single();
        }

        public List<DbTransaction> GetTransactions(Guid reference)
        {
            var account = GetAccount(reference);
            if (account == null)
            {
                throw new BankAccountException(ErrorCode.AccountDoesNotExists, "Reference does not correspond to any count");
            }
            return _context.Transaction.Where(t => t.Account.Reference == reference).ToList();
        }

        public bool DebitAccount(Guid reference, float amount, string label)
        {
            var account = GetAccount(reference);
            if (account == null)
            {
                throw new BankAccountException(ErrorCode.AccountDoesNotExists, "Reference does not correspond to any count");
            }
            if (GetBalance(reference) < amount)
            {
                throw new BankAccountException(ErrorCode.TransactionNotPossible, "Transaction Impossible due to insufficient credit");
            }
            _context.Transaction.Add(new DbTransaction()
            {
                Type = Domain.Entities.Transaction.TransactionType.WithdrawalMoney,
                Amount = amount,
                Label = label,
                Account = account
            });
            _context.SaveChanges();
            account.Balance -= amount;
            _context.Account.Update(account);
            _context.SaveChanges();
            return true;
        }
        public bool CreditAccount(Guid reference, float amount, string label)
        {
            var account = GetAccount(reference);
            if (account == null)
            {
                throw new BankAccountException(ErrorCode.AccountDoesNotExists, "Reference does not correspond to any count");
            }
            _context.Transaction.Add(new DbTransaction()
            {
                Type = Domain.Entities.Transaction.TransactionType.DepositMoney,
                Amount = amount,
                Label = label,
                Account = account
            });
            _context.SaveChanges();
            account.Balance += amount;
            _context.Account.Update(account);
            _context.SaveChanges();

            return true;
        }
        
    }
}
