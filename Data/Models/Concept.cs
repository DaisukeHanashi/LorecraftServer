namespace Lorecraft_API.Data.Models
{
    public record Concept 
    {
        public required string ConceptId { get; set; } 
        public required string Name { get; init; }
        public required long LoreId { get; init; }
        public required string Type { get; init; }
        public virtual Description[]? Descriptions { get; set; }

        public string[] Properties { get; }
        =
        [
            nameof(ConceptId),
            nameof(LoreId),
            nameof(Name),
            nameof(Type)
        ];

        
    }
}