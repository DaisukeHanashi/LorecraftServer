using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lorecraft_API.DependencyInjection;
using Lorecraft_API.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lorecraft_API.Helper
{
     public class CustomJwtDataFormat(
        string algorithm, 
        TokenValidationParameters validationParameters, 
        IConfiguration configuration, 
        TokenProviderOptionsFactory tokenProviderOptionsFactory) : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _algorithm = algorithm;
        private readonly TokenValidationParameters _validationParameters = validationParameters;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory = tokenProviderOptionsFactory;

        private readonly IConfiguration _configuration = configuration;

        public AuthenticationTicket? Unprotect(string? protectedText)
           => Unprotect(protectedText, null) ?? throw new NotImplementedException("Unprotect layer hasn't been implemented!");


        public AuthenticationTicket? Unprotect(string? protectedText, string? purpose)
        {
            JwtSecurityTokenHandler handler = new();
            try
            {
                ClaimsPrincipal principal = handler.ValidateToken(protectedText, _validationParameters, out SecurityToken validToken);

                if (validToken is not JwtSecurityToken validJwt)
                    throw new ArgumentException("Invalid JWT");

                if (!validJwt.Header.Alg.Equals(_algorithm, StringComparison.Ordinal))
                    throw new ArgumentException($"Algorithm must be '{_algorithm}'");

                
                return new AuthenticationTicket(principal, new AuthenticationProperties(), Constants.AuthenticationScheme);
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
        public string Protect(AuthenticationTicket ticket) => Protect(ticket, null);

        public string Protect(AuthenticationTicket ticket, string? purpose)
        {
            var token = _configuration.GetTokenAuthentication();
            var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(token.SecretKey));

            var claimsPrincipal = ticket.Principal;
            var identity = claimsPrincipal.Identity as ClaimsIdentity ?? throw new ArgumentException("Identity is not found!");
            var tokenProviderOptions = _tokenProviderOptionsFactory.Create(token, signingKey);
            var tokenProvider = new TokenProvider(Options.Create(tokenProviderOptions));
            var encodedJwt = tokenProvider.GetJwtSecurityToken(identity, tokenProviderOptions);
            return encodedJwt;
        }

    }
}