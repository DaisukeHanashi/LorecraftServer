namespace Lorecraft_API.Data.Models
{
    public record Context 
    {
        public required string ContextId { get; set; }
        public required string ContextValue { get; init; }
        public DateOnly? Date { get; init; }
        public string? CharacterId { get; init; }
        public string? CountryId { get; init; }
        public string? FactionId { get; init; }

        public string[] Properties { get; }
        = 
        [
            nameof(CharacterId),
            nameof(ContextId),
            nameof(FactionId),
            nameof(CountryId),
            nameof(ContextValue),
            nameof(Date)
        ];



    }
}