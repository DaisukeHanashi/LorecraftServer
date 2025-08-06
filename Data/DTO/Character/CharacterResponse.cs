namespace Lorecraft_API.Data.DTO.Character
{
    public record CharacterResponse
    {
        public required string CharacterId { get; set; }
        public required long LoreId { get; init; }
        public string? FactionId { get; init; }
        public string? CountryId { get; init; }
        public string? PersonId { get; init; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Nationality { get; set; }
        public string[] Personality { get; init; } = []; 
        public string? Background { get; init; }
        public string[] Purpose { get; init; } = []; 
        public string[] Passions { get; init; } = []; 
        public string[] Nickname { get; init; } = []; 
    }
}