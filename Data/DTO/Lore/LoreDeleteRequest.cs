namespace Lorecraft_API.Data.DTO.Lore
{
    public record LoreDeleteRequest
    {
        public required long LoreId { get; init; }
        public required string Password { get; init; }
    }
}