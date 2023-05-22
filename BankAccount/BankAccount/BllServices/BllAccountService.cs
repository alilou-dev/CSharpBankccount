using BankAccount.Domain.Entities.BankAccount;
using BankAccount.Domain.Exceptions;
using BankAccount.Domain.models;
using BankAccount.Domain.models.request;
using BankAccount.Domain.models.response;
using BankAccount.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection.Metadata.Ecma335;

namespace BankAccount.BllServices
{
    public class BllAccountService : IBllAccountService
    {
        private readonly IBankAccountDataAccessService _dataAccessService;    
        public BllAccountService(IBankAccountDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }

        public Response CreateAccount(string label)
        {
            Response result = new();
            DbAccount account = new()
            {
                Label = label,
                Reference = Guid.NewGuid(),
                Balance = 0
            };

            try
            {
                _dataAccessService.Create(account);
            }
            catch (Exception ex)
            {
                // for more tracability we can use the BankAccountException and return it here but the client do not need this kind of information so instead we will provide a simple ResponseCode As follow
                result.Errors.Add(new ResponseError(ResponseCode.DatabaseIssue, ex.Message));            
            }
            return result;
        }

        public Response GetAllAccount()
        {
            Response result = new();
            try
            {
                List<DbAccount> accounts = _dataAccessService.GetAllAccount();
                result = new Response(accounts.Select(a => new GetAccountInfoVM(a)).ToList());
            }

            catch (Exception ex)
            {
                result.Errors.Add(new ResponseError(ResponseCode.DatabaseIssue, ex.Message));
                return result;
            }

            return result;
        }

        public Response GetAccountInfo(Guid reference)
        {
            Response result = new();
            try
            {
                result = new Response(new GetAccountInfoVM(_dataAccessService.GetAccount(reference)));
            }

            catch (Exception ex)
            {
                result.Errors.Add(new ResponseError(ResponseCode.DatabaseIssue, ex.Message));
                return result;
            }

            return result;
        }

        public Response GetBalanceAccount(Guid reference)
        {
            Response result = new();
            try
            {
                var dbResult = _dataAccessService.GetBalance(reference);
                result = new Response(new GetAccountBalanceResponseVM(dbResult));
            }
            
            catch(Exception ex)
            {
                if (ex.Message == "Sequence contains no elements")
                {
                    result.Errors.Add(new ResponseError(ResponseCode.GetAccountBalanceInvalidAccountReference, "no account related to this reference"));
                    return result;
                }
                result.Errors.Add(new ResponseError(ResponseCode.DatabaseIssue, ex.Message));
                return result;
            }

            return result;

        }

        public Response DebitOrCreditAccount(CreditOrDebitAccountVM model, Guid accountRef)
        {
            bool isTransactionSucceeded = false;
            if (model.TransactionType != "d" && model.TransactionType != "c" )
            {
                return new Response(new List<ResponseError>() { new ResponseError(ResponseCode.DebitOrCreditAccountInvalidOperation, "Please provide d for debit and c for credit") });
            }
            
            Response result = new();
            try
            {
                if (model.TransactionType == "d")
                {
                    isTransactionSucceeded = _dataAccessService.DebitAccount(accountRef, model.Ammount, model.Label);
                }else
                {
                    isTransactionSucceeded = _dataAccessService.CreditAccount(accountRef, model.Ammount, model.Label);
                }
                
            }
            catch(Exception ex)
            {
                if (ex.GetType() == typeof(BankAccountException))
                {
                    var newEx = (BankAccountException) ex;
                    if (newEx.ErrorCode is ErrorCode.TransactionNotPossible)
                    {
                        result.Errors.Add(new ResponseError(ResponseCode.DebitOrCreditAccountTransactionNotPossible, newEx.Message));
                        return result;
                    }
                    if (newEx.ErrorCode is ErrorCode.AccountDoesNotExists)
                    {
                        result.Errors.Add(new ResponseError(ResponseCode.DebitOrCreditAccountInvalidAccountReference, newEx.Message));
                        return result;
                    }
                }
                result.Errors.Add(new ResponseError(ResponseCode.DatabaseIssue, ex.Message));
                return result;
            }
            return result;
        }

        public Response GetAllTransactions(Guid accountRef)
        {
            Response result = new();
            try
            {
                var transactions = _dataAccessService.GetTransactions(accountRef);
                return new Response(transactions.Select(t => new GetAllTransactionsVM(t)).ToList());
            }
            catch(Exception ex)
            {
                if (ex.GetType() == typeof(BankAccountException))
                {
                    var newEx = (BankAccountException) ex;
                    if (newEx.ErrorCode is ErrorCode.AccountDoesNotExists)
                    {
                        result.Errors.Add(new ResponseError(ResponseCode.GetAllTransactionsInvalidAccountReference, newEx.Message));
                        return result;
                    }
                }
                result.Errors.Add(new ResponseError(ResponseCode.DatabaseIssue, ex.Message));
                return result;
            }

            return result;
        }


    }
}
