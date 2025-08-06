namespace Lorecraft_API.Data.Models
{
    public record Character 
    {
        public required string CharacterId { get; set; }
        public required long LoreId { get; init; }
        public string? FactionId { get; init; }
        public string? CountryId { get; init; }
        public string? PersonId { get; init; }
        public string[] Personality { get; init; } = []; 
        public string? Background { get; init; }
        public string[] Purpose { get; init; } = []; 
        public string[] Passions { get; init; } = []; 
        public string[] Nickname { get; init; } = []; 
        public virtual Person? Person { get; set; }

        public string[] Properties { get; }
        = 
        [
            nameof(CharacterId),
            nameof(LoreId),
            nameof(FactionId),
            nameof(CountryId),
            nameof(PersonId),
            nameof(Personality),
            nameof(Background),
            nameof(Purpose),
            nameof(Passions),
            nameof(Nickname)
        ];

    }
}