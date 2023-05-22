using BankAccount.BllServices;
using BankAccount.Domain.Entities.BankAccount;
using BankAccount.Domain.Exceptions;
using BankAccount.Domain.models;
using BankAccount.Domain.models.request;
using BankAccount.Domain.models.response;
using BankAccount.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BankAccount.Test.Bll
{
    public class TestBllAccountService
    {
        private readonly Mock<IBankAccountDataAccessService> _mockDataAcccess = new Mock<IBankAccountDataAccessService>();

        [Fact]
        public void TestCreateAccount_GivenReferenceAlreadyExists_ReturnException()
        {
            _mockDataAcccess.Setup(m => m.Create(It.IsAny<DbAccount>())).Throws(new BankAccountException(ErrorCode.AccountAlreadyExist, "Account already exist in db"));
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.CreateAccount("FakeLabel");
            Assert.False(actual.Success);
            Assert.True(actual.Errors.Count == 1);
            Assert.Equal("Account already exist in db", actual.Errors[0].MessageError);
            Assert.Equal(ResponseCode.DatabaseIssue, actual.Errors[0].ResponseCode);
        }
        [Fact]
        public void TestGetAllAccount_GivenResultFromDb_ReturnObjectWithAccounts()
        {
            var firstRef = new Guid();
            var secondRef = new Guid();
            List<DbAccount> expectedDataResult = new()
            {
                new DbAccount() { Id = 1,Balance = 0, Label = "FirstAccount", Reference = firstRef},
                new DbAccount() { Id = 2, Balance = 123, Label = "SecondAccount", Reference = secondRef}
            };
            _mockDataAcccess.Setup(m => m.GetAllAccount()).Returns(expectedDataResult);
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.GetAllAccount();
            var data = (List<GetAccountInfoVM>)actual.Data;
            Assert.Equal(2, data.Count);
            Assert.True(actual.Success);
            
            Assert.All(data.Zip(expectedDataResult, (d, e) => (data: d, expected: e)), item =>
            {
                Assert.Equal(item.expected.Label, item.data.AccountLabel);
                Assert.Equal(item.expected.Balance, item.data.Balance);
                Assert.Equal(item.expected.Reference.ToString(), item.data.AccountRef);
            });


        }
        [Fact]
        public void TestGetAccountInfo_GivenResultFromDb_ReturnObjectWithAccount()
        {
            var accountReference = new Guid();
            DbAccount expectedAccount = new DbAccount()
            {
                Id = 1,
                Balance = 129,
                Label = "Label",
                Reference = accountReference
            };
            _mockDataAcccess.Setup(m => m.GetAccount(It.IsAny<Guid>())).Returns(expectedAccount);
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.GetAccountInfo(It.IsAny<Guid>());
            var data = (GetAccountInfoVM)actual.Data;
            Assert.True(actual.Success);
            Assert.Equal(expectedAccount.Label, data.AccountLabel);
            Assert.Equal(expectedAccount.Reference.ToString(), data.AccountRef);
            Assert.Equal(expectedAccount.Balance, data.Balance);
        }
        [Fact]
        public void TestGetAccountBalance_GivenExistingRefAccount_ReturnsObjecWithBlanceRelated()
        {
            var accountRef = new Guid();
            _mockDataAcccess.Setup(m => m.GetBalance(accountRef)).Returns(123);
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.GetBalanceAccount(accountRef);
            Assert.True(actual.Success);
            var data = (GetAccountBalanceResponseVM)actual.Data;
            Assert.Equal(123, data.Balance);
        }

        [Fact]
        public void TestGetAccountBalance_GivenNotExistingRefAccount_ReturnObjectWithErrorMessage()
        {
            _mockDataAcccess.Setup(m => m.GetBalance(It.IsAny<Guid>())).Throws(new Exception("Sequence contains no elements"));
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.GetBalanceAccount(It.IsAny<Guid>());
            Assert.False(actual.Success);
            Assert.True(actual.Errors.Count > 0);
            Assert.Equal(ResponseCode.GetAccountBalanceInvalidAccountReference, actual.Errors[0].ResponseCode);
            Assert.Equal("no account related to this reference", actual.Errors[0].MessageError);

        }

        [Fact]
        public void TestCreditOrDebitAccount_GivenInvalidOperation_ReturnObjectWithError()
        {
            CreditOrDebitAccountVM vm = new(){
                Ammount = 0,
                TransactionType = "v"
            };
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.DebitOrCreditAccount(vm, new Guid());
            Assert.False(actual.Success);
            Assert.True(actual.Errors.Count == 1);
            Assert.Equal("Please provide d for debit and c for credit", actual.Errors[0].MessageError);
            Assert.Equal(ResponseCode.DebitOrCreditAccountInvalidOperation,actual.Errors[0].ResponseCode);

        }
        [Fact]
        public void TestCreditOrDebitAccount_GivenResultFromDb_ReturnResult()
        {
            _mockDataAcccess.Setup(a => a.CreditAccount(new Guid(), 100, "label")).Returns(true);
            var service = new BllAccountService(_mockDataAcccess.Object );
            var actual = service.DebitOrCreditAccount(new CreditOrDebitAccountVM()
            {
                TransactionType = "c",
                Ammount = 100
            }, new Guid());
            Assert.True(actual.Success);
        }
        [Fact]
        public void TestCreditOrDebitAccount_GivenExceptionFromDb_ReturnObjectWithError()
        {
            _mockDataAcccess.Setup(a => a.DebitAccount(new Guid(), 100, "label")).Throws(new Exception("Some error message"));
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.DebitOrCreditAccount(new CreditOrDebitAccountVM()
            {
                TransactionType = "d",
                Ammount = 100
            }, new Guid());
            Assert.False(actual.Success);
            Assert.True(actual.Errors.Count == 1);
            Assert.Equal("Some error message", actual.Errors[0].MessageError);
            Assert.Equal(ResponseCode.DatabaseIssue, actual.Errors[0].ResponseCode);
        }

        [Fact]
        public void TestCreditOrDebitAccount_GivenAmmountSuperiorToBalance_ReturnObjectWithError()
        {
            _mockDataAcccess.Setup(a => a.DebitAccount(new Guid(), 100, "label")).Throws(new BankAccountException(ErrorCode.TransactionNotPossible, "Transaction Impossible due to insufficient credit"));
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.DebitOrCreditAccount(new CreditOrDebitAccountVM()
            {
                TransactionType = "d",
                Ammount = 100
            }, new Guid());
            Assert.False(actual.Success);
            Assert.True(actual.Errors.Count == 1);
            Assert.Equal("Transaction Impossible due to insufficient credit", actual.Errors[0].MessageError);
            Assert.Equal(ResponseCode.DebitOrCreditAccountTransactionNotPossible, actual.Errors[0].ResponseCode);
        }

        [Fact]
        public void TestGetAllTransactions_GivenAccountRefInvalid_ReturnObjectWithError()
        {
            _mockDataAcccess.Setup(a => a.GetTransactions(It.IsAny<Guid>())).Throws(new BankAccountException(ErrorCode.AccountDoesNotExists, "No related Account for this reference"));
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.GetAllTransactions(new Guid());
            Assert.False(actual.Success);
            Assert.Equal("No related Account for this reference", actual.Errors[0].MessageError);
            Assert.Equal(ResponseCode.GetAllTransactionsInvalidAccountReference, actual.Errors[0].ResponseCode);
        }

        [Fact]
        public void TestGetAllTransactions_GivenValidReference_ReturnObjectWithResult()
        {
            Guid reference = new();
            List<DbTransaction> expectedTransactions = new()
            {
                new DbTransaction
                {
                    Id = 1,
                    Amount = 100,
                    Label = "TestTransaction",
                    Account = new DbAccount()
                    {
                        Reference = reference
                    },
                    Type = Domain.Entities.Transaction.TransactionType.DepositMoney
                }
            };
            _mockDataAcccess.Setup(a => a.GetTransactions(reference)).Returns(expectedTransactions);
            var service = new BllAccountService(_mockDataAcccess.Object);
            var actual = service.GetAllTransactions(reference);
            Assert.True(actual.Success);
            Assert.True(actual.Data.GetType() == typeof(List<GetAllTransactionsVM>));
            var allTransaction = (List<GetAllTransactionsVM>)actual.Data;
            Assert.All(allTransaction.Zip(expectedTransactions, (d, e) => (data: d, expected: e)), item =>
            {
                Assert.Equal(item.expected.Label, item.data.Label);
                Assert.Equal(item.expected.Amount, item.data.Amount);
                Assert.Equal(item.expected.Account.Reference, item.data.AccountReference);
                Assert.Equal(item.expected.Type.ToString(), item.data.TransactionType);
            });

        }

    }
}