
namespace BankAccount.Domain.models
{
    public enum ResponseCode
    {
        Unknown = 0,
        CreateAccountInvalidPayload,
        GetAccountBalanceInvalidAccountReference,
        GetAccountInfoInvalidReference,
        DatabaseIssue,
        DebitOrCreditAccountInvalidAccountReference,
        GetAllAccountOperationsInvalidAccountReference,
        DebitOrCreditAccountInvalidOperation,
        DebitOrCreditAccountTransactionNotPossible,
        GetAllTransactionsInvalidAccountReference
        
    }
}
