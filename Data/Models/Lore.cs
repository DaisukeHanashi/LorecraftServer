namespace Lorecraft_API.Data.Models
{
    public record Lore 
    {
        public required long LoreId { get; set; }
        public required long AccountId { get; set; }
        public required string UniverseName { get; set; }
        public required bool IsPublic { get; set; }

        public string[] Properties { get; }
        = 
        [
            nameof(UniverseName),
            nameof(AccountId),
            nameof(LoreId),
            nameof(IsPublic)
        ];

    }
}