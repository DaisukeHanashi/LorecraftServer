namespace Lorecraft_API.Data.Models
{
    public record Account
    {
        public required long AccountId { get; set; }
        public string? PfpId { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required DateOnly Birthdate { get; set; }
        public required char Gender { get; set; }
        public bool IsEmailVerified { get; set; }
        public required string Email { get; set; }
        public required string ContactNum { get; set; }
        public bool IsContactVerified { get; set; }
        public bool IsSpammer { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public string? TempPassword { get; set; }
        public string? PenName { get; set; }
        public string? CountryCodeContact { get; set; }
        public DateTime DatetimeCreated { get; set; }
        public string[] Properties { get; } =
        [
         nameof(AccountId),
         nameof(PfpId),
         nameof(FirstName),
         nameof(MiddleName),
         nameof(LastName),
         nameof(Birthdate),
         nameof(Gender),
         nameof(IsEmailVerified),
         nameof(Email),
         nameof(ContactNum),
         nameof(IsContactVerified),
         nameof(IsSpammer),
         nameof(Password),
         nameof(Role),
         nameof(TempPassword),
         nameof(PenName),
         nameof(CountryCodeContact),
         nameof(DatetimeCreated)
        ];

    }
}