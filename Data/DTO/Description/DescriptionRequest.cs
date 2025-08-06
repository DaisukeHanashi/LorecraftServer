namespace Lorecraft_API.Data.DTO.Description
{
    public record DescriptionRequest
    {
        public required string ConceptId { get; init; }
        public required string Type { get; init; }
        public required string Content { get; init; }
        public int? OrderN { get; init; }

    }
    public record DescriptionUpdateRequest
    {
        public required string DescriptionId { get; init; }
        public string? ConceptId { get; init; }
        public string? Type { get; init; }
        public string? Content { get; init; }
        public int? OrderN { get; init; }
        public string[] GetParams()
        {
            List<string> parameters = [];

            if (!string.IsNullOrEmpty(ConceptId))
            {
                parameters.Add(nameof(ConceptId));
            }
            if (!string.IsNullOrEmpty(Type))
            {
                parameters.Add(nameof(Type));
            }

            if (!string.IsNullOrEmpty(Content))
            {
                parameters.Add(nameof(Content));
            }

            if (OrderN is not null)
            {
                parameters.Add(nameof(OrderN));
            }

            return [.. parameters];

        }

    }
}