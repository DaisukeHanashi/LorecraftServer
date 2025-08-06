namespace Lorecraft_API.Data.DTO.Account
{
    public record ContactRequest : IAccountSignInRequest
    {
        public required string ContactNum { get; init; }
        public required string Password { get; init; }
    }
}