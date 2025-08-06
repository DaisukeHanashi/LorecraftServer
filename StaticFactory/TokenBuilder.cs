using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lorecraft_API.Data.DTO.Account;
using Lorecraft_API.Helper;
using Lorecraft_API.Manager;
using Lorecraft_API.Resources;
using Microsoft.AspNetCore.Authentication;

namespace Lorecraft_API.StaticFactory
{
    public static class TokenBuilder
    {

        public static void SetAuthenticationCookies(this HttpContext context, string token){
            context.Response.Cookies.Append(Constants.Authorization, token, new CookieOptions{
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Expires = Constants.CurrentUtc8PlusNow.AddDays(5), 
                Secure = true
            });
        }

        public static async Task<(string?, ResultMessage)> ProduceToken(AuthenticationManager manager, 
        AccountResponse acc, 
        ISecureDataFormat<AuthenticationTicket> format){
           
           (AuthenticationTicket?, TokenProviderOptionsFactory?) result = await manager.SignIn(acc); 

           AuthenticationTicket? ticket = result.Item1; 

           TokenProviderOptionsFactory? factory = result.Item2; 

           if(ticket is null || factory is null)
             return(null, ResultMessageFactory.CreateInternalServerErrorResult("Authentication Process Failure")); 

           await ticket.SwitchTicketToAddMoreClaims(factory, acc); 

           string token = format.Protect(ticket); 

           return (token, ResultMessageFactory.CreateAcceptedResult("Login successful")); 

        }

        private static async Task<AuthenticationTicket> SwitchTicketToAddMoreClaims(this AuthenticationTicket ticket, TokenProviderOptionsFactory optionsFactory, AccountResponse acc){
            var oldPrincipal = ticket.Principal; 

            var newPrincipal = await oldPrincipal.AddClaims(optionsFactory, acc); 

            return SwitchTicket(ticket, newPrincipal); 
        }

         private static AuthenticationTicket SwitchTicket(this AuthenticationTicket oldTicket, ClaimsPrincipal principal) => new(principal, oldTicket.Properties, oldTicket.AuthenticationScheme);
         private static async Task<ClaimsPrincipal> AddClaims(this ClaimsPrincipal principal, TokenProviderOptionsFactory optionsFactory, AccountResponse acc){
            var options = optionsFactory.Create(); 

            DateTime now = DateTime.UtcNow;  

            ClaimsIdentity identity = principal.Identity as ClaimsIdentity ?? options.IdentityRefresher(acc); 

            var claims = new Claim[]
            {
        new (JwtRegisteredClaimNames.Sub, acc.AccountId.ToString()),
        new (JwtRegisteredClaimNames.Name, ConcatenateFullName(acc)),
        new (JwtRegisteredClaimNames.Jti, await options.NonceGenerator()),
        new (JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
            };

            identity.AddClaims(claims); 

            return new([identity]);

            
        }

        private static string ConcatenateFullName(AccountResponse human) => $"{human.FirstName} {human.LastName}"; 

        private static long ToUnixEpochDate(DateTime date) => new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
    }
}