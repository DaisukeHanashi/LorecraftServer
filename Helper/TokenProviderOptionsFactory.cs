using Lorecraft_API.Data.Repository;
using Lorecraft_API.DependencyInjection;
using Lorecraft_API.InternalModel;
using Lorecraft_API.Manager;
using Microsoft.IdentityModel.Tokens;

namespace Lorecraft_API.Helper
{
    public class TokenProviderOptionsFactory(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
          private readonly IConfiguration _configuration = configuration;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public TokenProviderOptions Create(TokenAuthentication token, SymmetricSecurityKey signingKey)
        {
            if (_httpContextAccessor.HttpContext == null)
                throw new ArgumentException("HttpContext is referenced as null!");

            IAccountRepository accountRepo = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IAccountRepository>();
            var authManager = new AuthenticationManager(_configuration, _httpContextAccessor, accountRepo);
            return new()
            {
                Path = token.TokenPath,
                Audience = token.Audience,
                Issuer = token.Issuer,
                Expiration = TimeSpan.FromDays(token.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature),
                IdentityResolver = authManager.GetClaimsIdentity,
                IdentityRefresher = authManager.CreateClaimsIdentity
            };
        }
         public TokenProviderOptions Create(){
            var token = _configuration.GetTokenAuthentication(); 
            var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(token.SecretKey));

            if (_httpContextAccessor.HttpContext == null)
                throw new ArgumentException("HttpContext is referenced as null!");

            IAccountRepository accountRepo = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IAccountRepository>();
            var authManager = new AuthenticationManager(_configuration, _httpContextAccessor, accountRepo);

                return new()
            {
                Path = token.TokenPath,
                Audience = token.Audience,
                Issuer = token.Issuer,
                Expiration = TimeSpan.FromDays(token.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature),
                IdentityResolver = authManager.GetClaimsIdentity,
                IdentityRefresher = authManager.CreateClaimsIdentity
            };

        }
    }
}