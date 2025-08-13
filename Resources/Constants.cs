using Lorecraft_API.Data.Models;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Resources
{
    public static class Constants
    {
            public enum PasswordRequestMode
            {
                NoPassword,
                RequirePassword
            };
        public enum AuthenticationMode
        {
            Email,
            PenName
        }
        public enum CharacterRepositoryMode
        {
            CharacterLore,
            CharacterFaction,
            CharacterCountry,
            CharacterOnly
        }
        public enum SortingMode
        {
            Descending,
            Ascending
        }
        public const string Issuer = "Lorecraft_is_crafting_lore";
        public const string AuthenticationScheme = "CraftLore";
        public const string Authorization = "Lorencraftu";
        public const string None = "Unknown";
        public const string Identifier = "UserID";
        public const string RegularUser = "Regular";
        public const string TooManyRetries = "TMR";
        public const string HasOrderNumberOnSQL = "order_n > 0";
        public static readonly ResultMessage ValueUnknown = ResultMessageFactory.CreateBadRequestResult("Value is unknown!");

        public const int DuplicateKey = 23505;
        public const int MaxRetries = 25;

        public static readonly TimeZoneInfo UtcPlus8Zone = TimeZoneInfo.CreateCustomTimeZone("UTC+08:00", TimeSpan.FromHours(8), "UTC+08", "UTC+08");
        public static DateTime CurrentUtc8PlusNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, UtcPlus8Zone);

        public static class ContentType
        {
            public const string ApplicationJson = "application/json";
            public const string ApplicationForm = "application/x-www-form-urlencoded";
            public const string MultiPart = "multipart/form-data";
        }
        public static class CorsSetting
        {
            public const string AllowLocal = "AllowLocalOrigin";
        }
        public static class CachePrefix
        {
            public const string AccountKey = "account";
            public const string CharacterKey = "character";
            public const string ConceptKey = "concept";
            public const string ContextKey = "context";
            public const string CountryKey = "country";
            public const string DescriptionKey = "description";
            public const string FactionKey = "faction";
            public const string LoreKey = "lore";
            public const string StoryKey = "story";
        }

        public static class CommonMessages
        {
            public const string LogoutMessage = "User has successfully been signed out!";
            public const string LogoutFailMessage = "Failed to logout!";
            public const string EmptySQL = "Empty SQL script is returned!";
            public const string IDNotFound = "ID (primary identifier of a certain identifier) is not found!";
            public const string IncorrectAuth = "Incorrect Username/Password";
            public const string SuccessfulLogin = "Login successful";
            public const string FunctionNotFound = "Function Not Found!";
            public const string ArgumentNotFound = "Argument either irrelevantly passed or not found!";
            public const string PersonNotFound = "Person data is not found!";
            public const string ObjectIdentificationError = "No object is identified properly";
            public const string ReadAction = "retrieved";
            public const string CreateAction = "created";
            public const string UpdateAction = "updated";
            public const string DeleteAction = "deleted";
            public const string ReadPresentAction = "retrieve";
            public const string CreatePresentAction = "create";
            public const string UpdatePresentAction = "update";
            public const string DeletePresentAction = "delete";
            public static string GetSuccessfulActMessage(string entityName, string action, bool isPlural) => !isPlural
            ? GetSuccessfulActMessage(entityName, action) : $"List of {entityName.ToLower() + 's'} have been successfully {action}!";
            public static string GetSuccessfulActMessage(string entityName, string action) => $"{entityName} is successfully {action}!";
            public static string GetFailureMessage(string entityName, string action) => $"Failed to {action} the {entityName.ToLower()}";
            public static string GetNotFoundMessage(string entityName) => $"{entityName} not found!";
        }
    }
}