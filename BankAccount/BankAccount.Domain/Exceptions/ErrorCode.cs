using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccount.Domain.Exceptions
{
    public enum ErrorCode
    {
        Unknown = 0,
        AccountAlreadyExist,
        TransactionNotPossible,
        AccountDoesNotExists
    }
}
