using System.Security.Cryptography;
using Lorecraft_API.Data.Models;



namespace Lorecraft_API.Manager
{
    public static class GeneratorManager
    {
        private static readonly Random rand = new();
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly string millis_frag = DateTime.Now.TimeOfDay.TotalMilliseconds.ToString("00")[^5..];
        private static readonly string now_year_frag = DateTime.Now.Year.ToString();
        private static readonly string now_month_frag = DateTime.Now.Month.ToString();
        private static readonly string now_day_frag = DateTime.Now.Day.ToString();

        private static byte[] GenerateRandomBytes(int anySize)
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[anySize];
            rng.GetBytes(bytes);
            return bytes;
        }

        private static string TenthsFrag() => rand.Next(10, 99).ToString();
        private static string HundredFrag() => rand.Next(100, 999).ToString();
        private static string ThousandFrag() => rand.Next(1000, 9999).ToString();
        private static string TenThousandFrag() => rand.Next(10000, 99999).ToString();
        private static string HundredThousandFrag() => rand.Next(100000, 999999).ToString();
        private static string RandomString(int length) => new([.. Enumerable.Repeat(chars, length).Select(s => s[rand.Next(s.Length)])]);
        public static long GenerateAccountId() => Convert.ToInt64(now_year_frag + now_month_frag + now_day_frag + millis_frag + ThousandFrag());
        public static long GenerateLoreId(string accID) => Convert.ToInt64(HundredFrag() + now_year_frag[2..] + accID[5..]);
        public static long GenerateStoryId(string loreID) => Convert.ToInt64(TenthsFrag() + now_year_frag[2..] + loreID[4..]);
        public static string GenerateFactionId(string loreID) => loreID[4..] + ThousandFrag();
        public static string GenerateStringId(string parentID) => string.Join('-', parentID[5..], RandomString(5));
        public static string GenerateStringId() => RandomString(10);
        public static string GeneratePublicString(int size = 32) => Convert.ToBase64String(GenerateRandomBytes(size));

        public static object ResetID(this object entity)
        {
            var generator = FindRightfulGenerator(entity);

            switch (entity)
            {
                case Account acc:
                    acc.AccountId = generator();
                    entity = acc;
                    break;

                case Lore lore:
                    lore.LoreId = generator(lore.AccountId.ToString());
                    entity = lore;
                    break;

                case Concept concept:
                    concept.ConceptId = generator();
                    entity = concept;
                    break;

                case Character character:
                    character.CharacterId = generator();
                    entity = character;
                    break;

                case Person person:
                    person.PersonId = generator();
                    entity = person;
                    break;

                case Story story:
                    story.StoryId = generator(story.StoryId.ToString());
                    entity = story;
                    break;

                case Context context:
                    context.ContextId = generator();
                    entity = context;
                    break;

                case Faction faction:
                    faction.FactionId = generator(faction.LoreId.ToString());
                    entity = faction;
                    break;

                case Description description:
                    description.DescriptionId = generator(description.ConceptId);
                    entity = description;
                    break;
                
                case Country country:
                    country.CountryId = generator(country.LoreId.ToString());
                    entity = country;
                    break;


                default:
                    throw new ArgumentException("Unsupported entity type", nameof(entity));

            }

            return entity;
        }

        private static dynamic FindRightfulGenerator(object entity) => entity switch
        {
            Account => (Func<long>)(() => GenerateAccountId()),
            Lore => (Func<string, long>)((id) => GenerateLoreId(id)),
            Story => (Func<string, long>)((id) => GenerateStoryId(id)),
            Faction => (Func<string, string>)((id) => GenerateFactionId(id)),
            Description or Country => (Func<string, string>)((id) => GenerateStringId(id)),
            Concept or Character or Person or Context => (Func<string>)(() => GenerateStringId()),
            _ => throw new ArgumentException("Unsupported entity type", nameof(entity))
        };

    }
}