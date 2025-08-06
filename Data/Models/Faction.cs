namespace Lorecraft_API.Data.Models
{
    public record Faction
    {
        public required string FactionId { get; set; }
        public required string Name { get; init; }
        public required string Type { get; init; }
        public required long LoreId { get; init; }
        public string? Purpose { get; init; }
        public string[] Properties { get; }
        =
        [
            nameof(Name),
            nameof(Type),
            nameof(FactionId),
            nameof(LoreId),
            nameof(Purpose)
        ];


    }
}