using System.Security.Claims;
using Lorecraft_API.Data.DTO.Account;
using static Lorecraft_API.Resources.Constants; 
using Lorecraft_API.Data.Repository.Interface;
using Lorecraft_API.DependencyInjection;
using Lorecraft_API.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Lorecraft_API.Helper;
using Lorecraft_API.Data.Repository;

namespace Lorecraft_API.Manager
{
    public class AuthenticationManager(IConfiguration configuration, IHttpContextAccessor httpContextAccessor
    , IAccountRepository accountRepository)
    {
        private readonly IConfiguration _configuration = configuration; 
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor; 
        private readonly IAccountRepository _accountRepository = accountRepository;

     

        public async Task<ClaimsIdentity> GetClaimsIdentity(IAccountSignInRequest authReq){
            var result = await _accountRepository.GetAccountThroughAuthentication(authReq, authReq is EmailRequest ? AuthenticationMode.Email : AuthenticationMode.PenName); 

            if(result?.Data is not AccountResponse acc_data)
              return await Task.FromResult<ClaimsIdentity>(null!); 

            ClaimsIdentity claimsIdentity = CreateClaimsIdentity(acc_data); 
            return await Task.FromResult(claimsIdentity); 
        }

        public bool CheckAndGetUserID(string identifier, out long accountID){
            accountID = 0; 
            
            if(!identifier.Equals(None) || identifier.All(char.IsDigit)){
                accountID = Convert.ToInt64(identifier); 
                return true;
            }

            return false;
        }

        public ClaimsIdentity CreateClaimsIdentity(AccountResponse acc){
            string accountID = acc.AccountId.ToString().Trim(); 
            string name = string.Join(" ", acc.FirstName[0] + "." + acc.FirstName[1], acc.LastName);
            string role = acc.Role.Trim();

            List<Claim> claims = [
                new(ClaimTypes.NameIdentifier, accountID, ClaimValueTypes.UInteger64, Issuer),
                new(ClaimTypes.Name, name, ClaimValueTypes.String, Issuer),
                new(ClaimTypes.Role, role, ClaimValueTypes.String, Issuer),
                new("UserID", accountID, ClaimValueTypes.UInteger64, Issuer),
                new("UserName", name, ClaimValueTypes.String, Issuer),
                new("UserRole", role, ClaimValueTypes.String, Issuer),
            ];

            return new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

        }
         public ClaimsPrincipal CreateClaimsPrincipal(ClaimsIdentity identity)
        {
            List<ClaimsIdentity> identities = [identity];
            return CreateClaimsPrincipal(identities);
        }
        private static ClaimsPrincipal CreateClaimsPrincipal(List<ClaimsIdentity> identities)
        {
            ClaimsPrincipal principal = new(identities);
            return principal;
        }
         public async Task<(AuthenticationTicket?, TokenProviderOptionsFactory?)> SignIn(AccountResponse acc, bool isPersistent = false)
        {
            var claimsIdentity = CreateClaimsIdentity(acc);
            var principal = CreateClaimsPrincipal(claimsIdentity);
            return (await SignIn(principal, isPersistent), _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<TokenProviderOptionsFactory>());
        } 

        public async Task<AuthenticationTicket?> SignIn(ClaimsPrincipal principal, bool isPersistent)
        {
            var token = _configuration.GetTokenAuthentication();

            if (_httpContextAccessor.HttpContext == null)
                return null; 
            
            try
            {
                var authProps = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddDays(token.ExpirationDays),
                        IsPersistent = isPersistent,
                        AllowRefresh = false
                    }; 
                    
                await _httpContextAccessor.HttpContext.SignInAsync(
                    Constants.AuthenticationScheme,
                    principal,
                    authProps);
                    
                return new(principal, authProps, Constants.AuthenticationScheme); 
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}