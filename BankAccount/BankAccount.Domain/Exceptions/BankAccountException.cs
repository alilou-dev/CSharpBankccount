
namespace BankAccount.Domain.Exceptions
{
    public class BankAccountException : Exception
    {
        public BankAccountException(ErrorCode code) : base()
        {
            ErrorCode = code;
        }

        public BankAccountException(ErrorCode code, string message) : base(message)
        {
            ErrorCode = code;
        }

        public virtual ErrorCode ErrorCode { get; }
    }
}
