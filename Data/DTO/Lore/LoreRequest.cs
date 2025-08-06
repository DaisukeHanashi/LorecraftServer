namespace Lorecraft_API.Data.DTO.Lore
{
    public record LoreRequest
    {
        public long? LoreId { get; init; }
        public required string UniverseName { get; init; }
        public required bool IsPublic { get; init; }
        public long? AccountId { get; set; }

        public string[] GetParams() =>
        [
            nameof(UniverseName),
            nameof(IsPublic)
        ];

    }
}