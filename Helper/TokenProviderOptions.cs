namespace Lorecraft_API.Helper
{
    public class TokenProviderOptions
    {
        public string Path { get; set; } = "api/token";
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
        public Microsoft.IdentityModel.Tokens.SigningCredentials SigningCredentials { get; set; } = null!;
        public Func<Data.DTO.Account.IAccountSignInRequest, Task<System.Security.Claims.ClaimsIdentity>> IdentityResolver { get; set; } = null!;

        public Func<Data.DTO.Account.AccountResponse, System.Security.Claims.ClaimsIdentity> IdentityRefresher {get; set; } = null!;                       

        public Func<Task<string>> NonceGenerator { get; set; }
          = () => Task.FromResult(Guid.NewGuid().ToString());
    }
}