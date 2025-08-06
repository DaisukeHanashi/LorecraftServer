namespace Lorecraft_API.InternalModel
{
    public class TokenAuthentication
    {
        public required string SecretKey { get; init; }
        public required string Audience { get; init; }
        public required string Issuer { get; init; }
        public required string TokenPath { get; init; }
        public required string CookieName { get; init; }
        public required int ExpirationMinutes { get; init; }
        public required int ExpirationDays { get; init; }
    }
}