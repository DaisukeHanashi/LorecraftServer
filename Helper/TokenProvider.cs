using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using static Lorecraft_API.Resources.Constants; 
namespace Lorecraft_API.Helper
{
    public class TokenProvider(IOptions<TokenProviderOptions> options)
    {
         private readonly TokenProviderOptions _options = options.Value;

        public string GetJwtSecurityToken(ClaimsIdentity identity, TokenProviderOptions tokenProvider)
        {
            DateTime now = DateTime.UtcNow;

            JwtSecurityToken jwt = new(
                issuer: tokenProvider.Issuer,
                audience: tokenProvider.Audience,
                claims: identity.Claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: tokenProvider.SigningCredentials
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
    }
}