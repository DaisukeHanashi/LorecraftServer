namespace Lorecraft_API.Data.DTO.Character
{
    public record PersonUpdateRequest
    {
        public required string PersonId { get; init; }
        public string? FirstName { get; init; }
        public string? MiddleName { get; init; }
        public string? LastName { get; init; }
        public DateOnly? Birthdate { get; init; }
        public string? Nationality { get; init; }

        public string[] GetParams()
        {
            List<string> parameters = [];

            if (!string.IsNullOrEmpty(FirstName))
                parameters.Add(nameof(FirstName));

            if (!string.IsNullOrEmpty(MiddleName))
                parameters.Add(nameof(MiddleName));

            if (!string.IsNullOrEmpty(LastName))
                parameters.Add(nameof(LastName));

            if (Birthdate is not null)
                parameters.Add(nameof(Birthdate));

            if (!string.IsNullOrEmpty(Nationality))
                parameters.Add(nameof(Nationality));

            return [.. parameters];

        }
    }
}