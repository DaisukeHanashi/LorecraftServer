using Lorecraft_API.Data.DTO.Account;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Data.Repository;
using Lorecraft_API.Data.Repository.Interface;
using Lorecraft_API.Manager;
using Lorecraft_API.Resources;
using Lorecraft_API.Resources.Handler;
using Lorecraft_API.Service;
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
    CookieTicketStore store,
    ICacheService cacheService) : ControllerBase
    {
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly AuthenticationManager _authManager = authManager;
        private readonly ISecureDataFormat<AuthenticationTicket> _ticketFormat = store.TicketFormat;
        private readonly ICacheService _cacheService = cacheService;

        [HttpGet("all")]
        public async Task<IActionResult> GetAccounts([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {

            string key = string.Join(':', CachePrefix.AccountKey, "all");
            var res = await _cacheService.GetAsync<ResultMessage>(key, _accountRepository.GetAccounts);
            var data = res.PaginateData<AccountResponse>(pageSize, pageNumber);
            return Ok(data);
        }
        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetAccount([FromHeader(Name = Constants.Identifier)] string identifier)
        {
            if (!_authManager.CheckAndGetUserID(identifier, out long accountID))
                return BadRequest(ValueUnknown);

            var accountResult = await _accountRepository.GetAccount(accountID, PasswordRequestMode.NoPassword);

            return accountResult.Code == StatusCodes.Status200OK ? Ok(accountResult) : NotFound(accountResult.Message);
        }

        [HttpPost("register")]
        [Consumes(ContentType.ApplicationJson)]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateRequest acc)
        {
            var res = await _accountRepository.CreateAccount(acc);
            await _cacheService.RemoveByPrefixAsync(CachePrefix.AccountKey);
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
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            var res = await _authManager.SignOut();

            return res.Code == StatusCodes.Status200OK ? Ok(res) : Conflict(res);
        }
        [HttpPatch("update/detail")]
        [Consumes(Constants.ContentType.ApplicationForm)]
        public async Task<IActionResult> Update([FromForm] AccountUpdateRequest req)
        {
            var res = await _accountRepository.UpdateAccount(req);
            await _cacheService.RemoveByPrefixAsync(CachePrefix.AccountKey);
            return res.Code == StatusCodes.Status200OK ? Ok(res.Message) : Conflict(res);
        }
        [HttpPatch("change/password")]
        [Consumes(Constants.ContentType.ApplicationForm)]

        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest req)
        {
            var res = await _accountRepository.ChangePassword(req);
            await _cacheService.RemoveByPrefixAsync(CachePrefix.AccountKey);
            return res.Code == StatusCodes.Status200OK ? Ok(res.Message) : BadRequest(res);
        }
        [HttpDelete("delete")]
        [Consumes(Constants.ContentType.ApplicationJson)]
        public async Task<IActionResult> Delete([FromBody] AccountDeleteRequest req)
        {
            var res = await _accountRepository.DeleteAccountSafe(req);
            await _cacheService.RemoveByPrefixAsync(CachePrefix.AccountKey);
            return res.Code == StatusCodes.Status200OK ? Ok(res.Message) : Conflict(res.Message);
        }


    }
}