using Lorecraft_API.Data.DTO.Account;
using Lorecraft_API.Data.Repository;
using Lorecraft_API.Data.Repository.Interface;
using Lorecraft_API.Manager;
using Lorecraft_API.Resources;
using Lorecraft_API.Resources.Handler;
using Lorecraft_API.StaticFactory;
using Lorecraft_API.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Lorecraft_API.Resources.Constants;

namespace Lorecraft_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Cors.EnableCors(CorsSetting.AllowLocal)]
    public class AccountController(IAccountRepository accountRepository,
    AuthenticationManager authManager,
    CookieTicketStore store) : ControllerBase
    {
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly AuthenticationManager _authManager = authManager;
        private readonly ISecureDataFormat<AuthenticationTicket> _ticketFormat = store.TicketFormat;

        [HttpGet("all")]
        public async Task<IActionResult> GetAccounts()
        {
            var res = await _accountRepository.GetAccounts();
            return Ok(res);
        }
        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetAccount([FromHeader(Name = Constants.Identifier)] string identifier)
        {
            if (!_authManager.CheckAndGetUserID(identifier, out long accountID))
                return BadRequest(Constants.ValueUnknown);

            var accountResult = await _accountRepository.GetAccount(accountID, Constants.PasswordRequestMode.NoPassword);

            return accountResult.Code == StatusCodes.Status200OK ? Ok(accountResult) : NotFound(accountResult.Message);
        }

        [HttpPost("register")]
        [Consumes(ContentType.ApplicationJson)]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateRequest acc)
        {
            var res = await _accountRepository.CreateAccount(acc);
            return res.Code == StatusCodes.Status201Created ? Ok(res) : BadRequest(res);
        }
        [HttpPost("login")]
        [Consumes(ContentType.ApplicationJson)]
        public async Task<IActionResult> SignIn([ModelBinder(typeof(AccountSignInBinder))] IAccountSignInRequest req)
        {
            var accountResult = await _accountRepository.GetAccountThroughAuthentication(req, req is EmailRequest ? AuthenticationMode.Email : AuthenticationMode.PenName);

            if (accountResult?.Data is not AccountResponse account)
                return Unauthorized(accountResult?.Message);

            var result = await TokenBuilder.ProduceToken(_authManager, account, _ticketFormat);

            string? token = result.Item1;

            if (token is null)
                return Conflict(result.Item2);

            HttpContext.SetAuthenticationCookies(token);

            return Accepted(result.Item2);
        }
        [HttpPost("test/login/{username}/{password}")]
        public async Task<IActionResult> TestLogin([FromRoute] string username, [FromRoute] string password)
        {
            var accountResult = await _accountRepository.GetAccountThroughAuthentication(new EmailRequest { Email = username, Password = password }, AuthenticationMode.Email);

            if (accountResult.Data is not AccountResponse acc)
                return Unauthorized(CommonMessages.IncorrectAuth);

            var result = await TokenBuilder.ProduceToken(_authManager, acc, _ticketFormat);

            string? token = result.Item1;

            if (token is null)
                return Conflict(result.Item2);

            HttpContext.SetAuthenticationCookies(token);

            return Accepted(new
            {
                access_token = token
            });
        }
        [HttpPatch("update/detail")]
        [Consumes(Constants.ContentType.ApplicationForm)]
        public async Task<IActionResult> Update([FromForm] AccountUpdateRequest req)
        {
            var res = await _accountRepository.UpdateAccount(req);

            return res.Code == StatusCodes.Status200OK ? Ok(res.Message) : Conflict(res);
        }
        [HttpPatch("change/password")]
        [Consumes(Constants.ContentType.ApplicationForm)]

        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest req)
        {
            var res = await _accountRepository.ChangePassword(req);

            return res.Code == StatusCodes.Status200OK ? Ok(res.Message) : BadRequest(res);
        }
        [HttpDelete("delete")]
        [Consumes(Constants.ContentType.ApplicationJson)]
        public async Task<IActionResult> Delete([FromBody] AccountDeleteRequest req)
        {
            var res = await _accountRepository.DeleteAccountSafe(req);

            return res.Code == StatusCodes.Status200OK ? Ok(res.Message) : Conflict(res.Message);
        }


    }
}