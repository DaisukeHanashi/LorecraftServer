namespace Lorecraft_API.Data.DTO.Character
{
    public record CharacterRequest
    {
        public required long LoreId { get; init; }
        public string? FactionId { get; init; }
        public string? CountryId { get; init; }
        public string? PersonId { get; set; }
        public string[] Personality { get; init; } = [];
        public string? Background { get; init; }
        public string[] Purpose { get; init; } = [];
        public string[] Passions { get; init; } = [];
        public string[] Nickname { get; init; } = [];
        public required string FirstName { get; init; }
        public string? MiddleName { get; init; }
        public required string LastName { get; init; }
        public required DateOnly Birthdate { get; init; }
        public string? Nationality { get; init; }
    }
    public record CharacterDeleteRequest
    {
        public required string CharacterId { get; init; }
        public required string PersonId { get; init; }
    }
}