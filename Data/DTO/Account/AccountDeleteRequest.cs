namespace Lorecraft_API.Data.DTO.Account
{
    public record AccountDeleteRequest
    {
        public required long AccountId { get; init; }
        public required string Password { get; init; }

    }
}