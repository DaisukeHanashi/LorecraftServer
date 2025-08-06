namespace Lorecraft_API.Data.DTO.Account
{
    public record PenNameRequest : IAccountSignInRequest
    {
        public required string PenName { get; init; }
        public required string Password { get; init; }
    }
}