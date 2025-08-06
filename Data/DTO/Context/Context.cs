namespace Lorecraft_API.Data.DTO.Context
{
    public record ContextRequest
    {
        public required string ContextValue { get; init; }
        public DateOnly? Date { get; init; }
        public string? CharacterId { get; init; }
        public string? CountryId { get; init; }
        public string? FactionId { get; init; }

    }
    public record ContextUpdateRequest
    {
        public required string ContextId { get; init; }
        public string? ContextValue { get; init; }
        public DateOnly? Date { get; init; }
        public string? CharacterId { get; init; }
        public string? CountryId { get; init; }
        public string? FactionId { get; init; }
        public string[] GetParams()
        {
            List<string> parameters = [];

            if (!string.IsNullOrEmpty(CharacterId))
            {
                parameters.Add(nameof(CharacterId));
            }

            if (!string.IsNullOrEmpty(ContextValue))
            {
                parameters.Add(nameof(ContextValue));
            }

            if (!string.IsNullOrEmpty(CountryId))
            {
                parameters.Add(nameof(CountryId));
            }

            if (Date is not null)
            {
                parameters.Add(nameof(Date));
            }

            if (!string.IsNullOrEmpty(FactionId))
            {
                parameters.Add(nameof(FactionId));
            }


            return [.. parameters];
        }
    }
}