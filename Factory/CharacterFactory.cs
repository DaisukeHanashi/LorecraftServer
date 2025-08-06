using Lorecraft_API.Data.DTO.Character;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Manager;
using Lorecraft_API.Resources;

namespace Lorecraft_API.Factory
{
    public interface ICharacterFactory
    {
        Character CreateCharacter(CharacterRequest req);
        Person CreatePerson(CharacterRequest req);
        PersonUpdateRequest? MapPersonUpdate(CharacterUpdateRequest req);
        CharacterResponse MapCharacterResponse(Character character, Person? person);
    }
    public class CharacterFactory : ICharacterFactory
    {
        public Character CreateCharacter(CharacterRequest req) => new()
        {
            CharacterId = GeneratorManager.GenerateStringId(),
            LoreId = req.LoreId,
            FactionId = req.FactionId,
            CountryId = req.CountryId,
            PersonId = req.PersonId,
            Passions = req.Passions,
            Personality = req.Personality,
            Background = req.Background,
            Nickname = req.Nickname,
            Purpose = req.Purpose
        };
        public Person CreatePerson(CharacterRequest req) => new()
        {
            PersonId = GeneratorManager.GenerateStringId(),
            FirstName = req.FirstName,
            MiddleName = req.MiddleName,
            LastName = req.LastName,
            Birthdate = req.Birthdate,
            Nationality = req.Nationality
        };
        
        public PersonUpdateRequest? MapPersonUpdate(CharacterUpdateRequest req) => !string.IsNullOrEmpty(req.PersonId) ? new()
        {
            PersonId = req.PersonId,
            FirstName = req.FirstName,
            MiddleName = req.MiddleName,
            LastName = req.LastName,
            Birthdate = req.Birthdate,
            Nationality = req.Nationality
        } : null;

        public CharacterResponse MapCharacterResponse(Character character, Person? person) => new()
        {
            PersonId = character.PersonId,
            FirstName = person?.FirstName ?? Constants.None,
            MiddleName = person?.MiddleName,
            LastName = person?.LastName ?? Constants.None,
            Birthdate = person?.Birthdate,
            Nationality = person?.Nationality ?? Constants.None,
            CharacterId = character.CharacterId,
            LoreId = character.LoreId,
            FactionId = character.FactionId,
            CountryId = character.CountryId,
            Passions = character.Passions,
            Personality = character.Personality,
            Background = character.Background,
            Nickname = character.Nickname,
            Purpose = character.Purpose
        };

        public void UpdatePerson(CharacterUpdateRequest req, ref Person person)
        {
            if (!string.IsNullOrEmpty(req.FirstName))
                person.FirstName = req.FirstName;

        }
    }
}