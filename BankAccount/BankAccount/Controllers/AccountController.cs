using BankAccount.Domain.models;
using BankAccount.Domain.models.request;
using BankAccount.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BankAccount.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBllAccountService _bllAccountService;

        public AccountController(IBllAccountService bllAccountService)
        {
            _bllAccountService = bllAccountService;
        }

        [HttpPost("/account/post")]
        public IActionResult CreateAccount([FromBody]CreateAccountVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BuildModelStateErrors(ModelState));
            }
            var result = _bllAccountService.CreateAccount(vm.Label);
            if (!result.Success)
            {
                return Json(result);
            }

            return StatusCode(201, "Account Created");
        }

        [HttpGet("/account/{accountRef}")]
        public IActionResult GetAccountInfo(string accountRef)
        {
            if (!Guid.TryParse(accountRef, out Guid reference))
            {
                return BadRequest(new Response(new List<ResponseError>() { new ResponseError(ResponseCode.GetAccountBalanceInvalidAccountReference, "please provide a valid Guid as reference") }));
            }

            return Json(_bllAccountService.GetAccountInfo(reference));
        }

        [HttpGet("/account")]
        public IActionResult GetAllAccount()
        {
            return Json(_bllAccountService.GetAllAccount());
        }

        [HttpGet("/account/{accountRef}/balance")]
        public IActionResult CheckAccountBalance(string accountRef)
        {
            if (!Guid.TryParse(accountRef, out Guid reference))
            {
                return BadRequest(new Response(new List<ResponseError>() { new ResponseError(ResponseCode.GetAccountBalanceInvalidAccountReference, "please provide a valid Guid as reference of the account") }));
            }
            return Ok(_bllAccountService.GetBalanceAccount(reference));
        }

        [HttpPost("/account/{accountRef}/transaction")]
        public IActionResult DebitOrCreditAccount(string accountRef, [FromBody] CreditOrDebitAccountVM vm)
        {
            if (!Guid.TryParse(accountRef, out Guid reference))
            {
                return BadRequest(new Response(new List<ResponseError>() { new ResponseError(ResponseCode.DebitOrCreditAccountInvalidAccountReference, "please provide a valid Guid as reference of the account") }));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(BuildModelStateErrors(ModelState));
            }
            var result = _bllAccountService.DebitOrCreditAccount(vm, reference);
            return Json(result);
        }

        [HttpGet("/account/{accountRef}/transactions")]
        public IActionResult GetAllAccountOperations(string accountRef) 
        {
            if (!Guid.TryParse(accountRef, out Guid reference))
            {
                return BadRequest(new Response(new List<ResponseError>() { new ResponseError(ResponseCode.GetAllAccountOperationsInvalidAccountReference, "please provide a valid Guid as reference of the account") }));
            }
            return Json(_bllAccountService.GetAllTransactions(reference));
        }

        #region PRIVATE

        private List<ModelErrorCollection?> BuildModelStateErrors(ModelStateDictionary ms)
        {
            return ms.Select(a => a.Value?.Errors).Where(er => er?.Count > 0).ToList();
        }
#endregion
    }

}
