namespace Lorecraft_API.Data.Models
{
    public record Description 
    {
        public required string DescriptionId { get; set; }
        public required string ConceptId { get; init; }
        public required string Type { get; init; }
        public required string Content { get; init; }
        public int? OrderN { get; init; }

        public string[] Properties { get; }
        =
        [
            nameof(DescriptionId),
            nameof(ConceptId),
            nameof(Type),
            nameof(Content),
            nameof(OrderN)
        ];



    }
}