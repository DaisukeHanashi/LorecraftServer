using System.Security.Claims;
using Lorecraft_API.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Lorecraft_API.Helper
{
    public class TokenValidationParametersFactory(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration; 
        public TokenValidationParameters Create(){
            var tokenAuthentication = _configuration.GetTokenAuthentication(); 
            SymmetricSecurityKey signingKey = new(System.Text.Encoding.UTF8.GetBytes(tokenAuthentication.SecretKey)); 

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = tokenAuthentication.Issuer,
                ValidAudience = tokenAuthentication.Audience,   
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role,
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256Signature]
            }; 
        }
    }
}