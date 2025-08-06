namespace Lorecraft_API.Data.DTO.Account
{
    public record ChangePasswordRequest
    {
        public required long AccountId { get; init; }
        public required string OldPassword { get; init; }
        public required string NewPassword { get; set; }
    }
}