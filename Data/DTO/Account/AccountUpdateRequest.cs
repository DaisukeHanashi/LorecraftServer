
namespace Lorecraft_API.Data.DTO.Account
{
    public record AccountUpdateRequest : IAccountPostOrPutRequest
    {
        public required long AccountId { get; init; }
        public string? FirstName { get; init; }
        public string? MiddleName { get; init; }
        public string? LastName { get; init; }
        public DateOnly? Birthdate { get; init; }
        public char? Gender { get; init; }
        public string Email { get; init; }
        public string ContactNum { get; init; }
        public string? PenName { get; init; }
        public string? Role { get; init; }
        public string? CountryCode { get; init; }

        public string[] GetParams()
        {
            List<string> parameters = [];

            if (!string.IsNullOrEmpty(FirstName))
            {
                parameters.Add(nameof(FirstName));
            }

            if (!string.IsNullOrEmpty(MiddleName))
            {
                parameters.Add(nameof(MiddleName));
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                parameters.Add(nameof(LastName));
            }

            if (Birthdate is not null)
            {
                parameters.Add(nameof(Birthdate));
            }

            if (Gender is not null)
            {
                parameters.Add(nameof(Gender));
            }

            if (!string.IsNullOrEmpty(Email))
            {
                parameters.Add(nameof(Email));
            }

            if (!string.IsNullOrEmpty(ContactNum))
            {
                parameters.Add(nameof(ContactNum));
            }

            if (!string.IsNullOrEmpty(PenName))
            {
                parameters.Add(nameof(PenName));
            }

            if (!string.IsNullOrEmpty(Role))
            {
                parameters.Add(nameof(Role));
            }

            return [.. parameters];
        }
    }
}