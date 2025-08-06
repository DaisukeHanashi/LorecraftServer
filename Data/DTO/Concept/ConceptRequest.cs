namespace Lorecraft_API.Data.DTO.Concept
{
    public record ConceptRequest
    {
        public string? ConceptId { get; init; }
        public required string Name { get; init; }
        public required long LoreId { get; init; }
        public required string Type { get; init; }

        public string[] GetParams() =>
        [
            nameof(Name),
            nameof(LoreId),
            nameof(Type)
        ];

    }
}