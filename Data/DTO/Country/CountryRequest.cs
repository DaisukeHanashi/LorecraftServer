namespace Lorecraft_API.Data.DTO.Country
{
    public record CountryRequest
    {
        public required long LoreId { get; set; }
        public required string Name { get; set; }
        public required string Continent { get; set; }
        public string? GovernmentType { get; set; }
    }
    public record CountryUpdateRequest
    {
        public required string CountryId { get; set; }
        public long? LoreId { get; set; }
        public string? Name { get; set; }
        public string? Continent { get; set; }
        public string? GovernmentType { get; set; }
        public string[] GetParams()
        {
            List<string> parameters = [];
            if (LoreId is not null)
            {
                parameters.Add(nameof(LoreId));
            }
            if (!string.IsNullOrEmpty(Name))
            {
                parameters.Add(nameof(Name));
            }
            if (!string.IsNullOrEmpty(Continent))
            {
                parameters.Add(nameof(Continent));
            }
            if (!string.IsNullOrEmpty(GovernmentType))
            {
                parameters.Add(nameof(GovernmentType));
            }

            return [.. parameters];
        }
    }
}