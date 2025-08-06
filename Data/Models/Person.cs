namespace Lorecraft_API.Data.Models
{
    public record Person 
    {
        public required string PersonId { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required DateOnly Birthdate { get; set; }
        public string? Nationality { get; set; }

        public string[] Properties { get; } = 
        [
         nameof(PersonId),
         nameof(Birthdate),
         nameof(FirstName),
         nameof(MiddleName),
         nameof(LastName),
         nameof(Birthdate),
         nameof(Nationality)
        ];



    }
}