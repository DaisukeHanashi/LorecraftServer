namespace Lorecraft_API.Data.DTO.Faction
{
    public record FactionRequest
    {
        public required string Name { get; init; }
        public required string Type { get; init; }
        public required long LoreId { get; init; }
        public string? Purpose { get; init; }
    }
    public record FactionUpdateRequest
    {
        public required string FactionId { get; init; }
        public string? Name { get; init; }
        public string? Type { get; init; }
        public string? Purpose { get; init; }
        public string[] GetParams()
        {
            List<string> parameters = [];

            if (!string.IsNullOrEmpty(Name))
            {
                parameters.Add(nameof(Name));
            }

            if (!string.IsNullOrEmpty(Type))
            {
                parameters.Add(nameof(Type));
            }

            if (!string.IsNullOrEmpty(Purpose))
            {
                parameters.Add(nameof(Purpose));
            }


            return [.. parameters];
        }
    }
}