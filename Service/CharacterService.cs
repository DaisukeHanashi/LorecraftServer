using Lorecraft_API.Data.DTO.Character;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Data.Repository;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;
using static Lorecraft_API.Resources.Constants;

namespace Lorecraft_API.Service
{
    public interface ICharacterService
    {
        Task<ResultMessage> CreateCharacter(CharacterRequest request);
        Task<ResultMessage> UpdateCharacter(CharacterUpdateRequest request);
        Task<ResultMessage> GetCharacters(object request, CharacterRepositoryMode mode);
        Task<ResultMessage> DeleteCharacterOnce(string characterID);
    }
    public class CharacterService(ICharacterRepository characterRepository
    , IPersonRespository personRespository, ICharacterFactory characterFactory) : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository = characterRepository;
        private readonly IPersonRespository _personRespository = personRespository;
        private readonly ICharacterFactory _characterFactory = characterFactory;
        public async Task<ResultMessage> GetCharacters(object request, CharacterRepositoryMode mode)
        {
            ResultMessage result = mode switch
            {
                CharacterRepositoryMode.CharacterLore => request is long loreID ? await _characterRepository.GetCharactersFromLore(loreID)
                : ResultMessageFactory.CreateNotFoundResult(CommonMessages.ArgumentNotFound),
                CharacterRepositoryMode.CharacterFaction => request is string factionID ? await _characterRepository.GetCharactersFromFaction(factionID)
                : ResultMessageFactory.CreateNotFoundResult(CommonMessages.ArgumentNotFound),
                CharacterRepositoryMode.CharacterCountry => request is string countryID ? await _characterRepository.GetCharactersFromCountry(countryID)
                : ResultMessageFactory.CreateNotFoundResult(CommonMessages.ArgumentNotFound),

                CharacterRepositoryMode.CharacterOnly => request is string characterID ? await _characterRepository.GetCharacter(characterID)
                : ResultMessageFactory.CreateNotFoundResult(CommonMessages.ArgumentNotFound),

                _ => ResultMessageFactory.CreateNotFoundResult(CommonMessages.FunctionNotFound)
            };

            if (result.IsStatusCodeNotAllOK())
                return result;

            var characterData = result.Data;

            result.ModifyData(characterData is Character[] groupOfCharacters
            ? groupOfCharacters.Select(@char => _characterFactory.MapCharacterResponse(@char, @char.Person))
            : characterData is Character singleCharacter
            ? _characterFactory.MapCharacterResponse(singleCharacter, singleCharacter.Person) 
            : null);

            return result;

        }
        public async Task<ResultMessage> CreateCharacter(CharacterRequest request)
        {
            var personResult = await _personRespository.CreatePerson(request);

            string? personId = personResult.IdString;

            if (string.IsNullOrEmpty(personId) && personResult.IsStatusCodeNotAllOK())
                return personResult;

            request.PersonId = string.Intern(personId!);

            return await _characterRepository.CreateCharacter(request);

        }
        public async Task<ResultMessage> UpdateCharacter(CharacterUpdateRequest request)
        {
            var personResult = await _personRespository.UpdatePerson(request);

            if (personResult.IsStatusCodeNotAllOK())
                return personResult;

            return await _characterRepository.UpdateCharacter(request);
        }

        public async Task<ResultMessage> DeleteCharacterOnce(string characterID)
        {
            var result = await _characterRepository.GetCharacter(characterID);

            if (result.IsStatusCodeNotAllOK() || result.Data is not Character figure)
                return result;

            return string.IsNullOrEmpty(figure.PersonId) ? await _characterRepository.DeleteCharacter(characterID) :
            await _characterRepository.DeleteCharacterAndPerson(new CharacterDeleteRequest { CharacterId = characterID, PersonId = figure.PersonId });
        }
    }
}