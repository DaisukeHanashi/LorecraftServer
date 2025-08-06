namespace Lorecraft_API.Data.DTO.Character
{
    public record CharacterUpdateRequest
    {
        public required long LoreId { get; init; }
        public required string CharacterId { get; init; }
        public string? FactionId { get; init; }
        public string? CountryId { get; init; }
        public string? PersonId { get; set; }
        public string[] Personality { get; init; } = [];
        public string? Background { get; init; }
        public string[] Purpose { get; init; } = [];
        public string[] Passions { get; init; } = [];
        public string[] Nickname { get; init; } = [];
        public string? FirstName { get; init; }
        public string? MiddleName { get; init; }
        public string? LastName { get; init; }
        public DateOnly? Birthdate { get; init; }
        public string? Nationality { get; init; }

        public string[] GetParams()
        {
            List<string> parameters = [];

            if (!string.IsNullOrEmpty(CharacterId))
            {
                parameters.Add(nameof(CharacterId));
            }

            if (!string.IsNullOrEmpty(FactionId))
            {
                parameters.Add(nameof(FactionId));
            }

            if (!string.IsNullOrEmpty(CountryId))
            {
                parameters.Add(nameof(CountryId));
            }

            if (!string.IsNullOrEmpty(PersonId))
            {
                parameters.Add(nameof(PersonId));
            }

            if (Personality != null && Personality.Length != 0)
            {
                parameters.Add(nameof(Personality));
            }

            if (Purpose != null && Purpose.Length != 0)
            {
                parameters.Add(nameof(Purpose));
            }

            if (!string.IsNullOrEmpty(Background))
            {
                parameters.Add(nameof(Background));
            }

            if (Passions != null && Passions.Length != 0)
            {
                parameters.Add(nameof(Passions));
            }

            if (Nickname != null && Nickname.Length != 0)
            {
                parameters.Add(nameof(Nickname));
            }

            return [.. parameters];
        }
    }
}