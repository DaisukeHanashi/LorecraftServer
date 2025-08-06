namespace Lorecraft_API.Data.DTO.Story
{
    public record StoryRequest
    {
        public long? StoryId { get; set; }
        public required long LoreId { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required int GenreId { get; init; }
        public string? DocLink { get; init; }
    }

    public record StoryUpdateRequest
    {
        public required long StoryId { get; set; }
        public long? LoreId { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public int? GenreId { get; init; }
        public string? DocLink { get; init; }
        public string[] GetParams()
        {
            List<string> parameters = [];

            if (LoreId is not null)
            {
                parameters.Add(nameof(LoreId));
            }

            if (!string.IsNullOrEmpty(Title))
            {
                parameters.Add(nameof(Title));
            }

            if (!string.IsNullOrEmpty(Description))
            {
                parameters.Add(nameof(Description));
            }

            if (GenreId is not null)
            {
                parameters.Add(nameof(GenreId));
            }

            if (!string.IsNullOrEmpty(DocLink))
            {
                parameters.Add(nameof(DocLink));
            }


            return [.. parameters];
        }
    }
}