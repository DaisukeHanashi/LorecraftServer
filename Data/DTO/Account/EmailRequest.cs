namespace Lorecraft_API.Data.DTO.Account
{
    public record EmailRequest : IAccountSignInRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}