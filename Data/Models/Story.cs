namespace Lorecraft_API.Data.Models
{
    public record Story
    {
        public required long StoryId { get; set; }
        public required long LoreId { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required int GenreId { get; init; }
        public string? DocLink { get; init; }
        public string[] Properties { get; } = 
        [
         nameof(StoryId),
         nameof(LoreId),
         nameof(Title),
         nameof(Description),
         nameof(GenreId),
         nameof(DocLink)
        ];
    }
}