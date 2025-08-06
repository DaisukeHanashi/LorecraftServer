namespace Lorecraft_API.Data.Models
{
    public record Country
    {
        public required string CountryId { get; set; }
        public required long LoreId { get; set; }
        public required string Name { get; set; }
        public required string Continent { get; set; }
        public string? GovernmentType { get; set; }
        public string[] Properties { get; }
        =
        [
            nameof(Name),
            nameof(GovernmentType),
            nameof(CountryId),
            nameof(LoreId),
            nameof(Continent)
        ];
        
    }
}