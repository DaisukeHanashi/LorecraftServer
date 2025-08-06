namespace Lorecraft_API.Data.DTO.Account
{

    public interface IAccountPostOrPutRequest
    {
        string Email { get; init; }
        string? PenName { get; init; }
        string ContactNum { get; init; }
    }
    public record AccountCreateRequest : IAccountPostOrPutRequest
    {
        public required string FirstName { get; init; }
        public string? MiddleName { get; init; }
        public required string LastName { get; init; }
        public required DateOnly Birthdate { get; init; }
        public required char Gender { get; init; }
        public required string Email { get; init; }
        public required string ContactNum { get; init; }
        public required string Password { get; init; }
        public string? PenName { get; init; }
        public string? CountryCode { get; init;  }
    }
}