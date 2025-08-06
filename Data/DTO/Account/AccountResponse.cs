namespace Lorecraft_API.Data.DTO.Account
{
    public record AccountResponse
    {
        public required long AccountId { get; init; }
        public required string FirstName { get; init; }
        public string? MiddleName { get; init; }
        public required string LastName { get; init; }
        public required DateOnly Birthdate { get; init; }
        public required char Gender { get; init; }
        public bool IsEmailVerified { get; init; }
        public required string Email { get; init; }
        public required string ContactNum { get; init; }
        public bool IsContactVerified { get; init; }
        public bool IsSpammer { get; init; }
        public required string Role { get; init; }
        public string? PenName { get; init; }
        public string? Password { get; init; }
        public string? CountryCode { get; init;  }


    }
}