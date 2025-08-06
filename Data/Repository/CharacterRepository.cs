using Lorecraft_API.Data.DTO.Character;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Data.Repository
{
    public interface ICharacterRepository
    {
        Task<ResultMessage> CreateCharacter(CharacterRequest request);
        Task<ResultMessage> GetCharacter(string characterID);
        Task<ResultMessage> GetCharactersFromLore(long loreID);
        Task<ResultMessage> GetCharactersFromFaction(string factionID);
        Task<ResultMessage> GetCharactersFromCountry(string countryID);
        Task<ResultMessage> UpdateCharacter(CharacterUpdateRequest characterUpdateRequest);
        Task<ResultMessage> DeleteCharacterAndPerson(CharacterDeleteRequest request);
        Task<ResultMessage> DeleteCharacter(string characterID);
    }
    public class CharacterRepository(SqlConnectionFactory sqlConnectionFactory
    , ILoggerFactory loggerFactory
    , ICharacterFactory characterFactory
    )
    : BaseRepository(sqlConnectionFactory, loggerFactory), ICharacterRepository
    {
        private readonly ICharacterFactory _characterFactory = characterFactory;
        private readonly string RetrievalMessage = "List of Characters has been successfully retrieved";

        public async Task<ResultMessage> CreateCharacter(CharacterRequest request)
        {
            try
            {
                Character character = _characterFactory.CreateCharacter(request);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(character, character.Properties);

                await ExecuteAsync(sql, character);
                return ResultMessageFactory.CreateCreatedResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Character),
                 Constants.CommonMessages.CreateAction));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Character), Constants.CommonMessages.CreatePresentAction));
                _logger.LogError(ex, message);

                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }

        }
        public async Task<ResultMessage> GetCharacter(string characterID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(characterID).AsNoTracking();

            Character? datum = await GetAsync<Character>(sql, new { CharacterId = characterID });

            return datum is null ? ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Character)))
            : ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Character), Constants.CommonMessages.ReadAction), datum);
        }
        public async Task<ResultMessage> GetCharactersFromLore(long loreID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityAndJoinForDefaultSchema(nameof(Character), [nameof(Character.LoreId), nameof(Character.PersonId)]).AsNoTracking();

            IEnumerable<Character> data = await GetJointListAsync(sql, new { LoreId = loreID }, string.Join(',', nameof(Character.CharacterId), nameof(Person.PersonId)), SQLWriter.CharacterAndPerson);

            return ResultMessageFactory.CreateOKResult(RetrievalMessage, data);
        }
        public async Task<ResultMessage> GetCharactersFromFaction(string factionID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityAndJoinForDefaultSchema(nameof(Character), [nameof(Character.FactionId), nameof(Character.PersonId)]).AsNoTracking();

            IEnumerable<Character> data = await GetJointListAsync(sql, new { FactionId = factionID }, string.Join(',', nameof(Character.CharacterId), nameof(Person.PersonId)), SQLWriter.CharacterAndPerson);

            return ResultMessageFactory.CreateOKResult(RetrievalMessage, data);
        }
        public async Task<ResultMessage> GetCharactersFromCountry(string countryID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityAndJoinForDefaultSchema(nameof(Character), [nameof(Character.CountryId), nameof(Character.PersonId)]).AsNoTracking();

            IEnumerable<Character> data = await GetJointListAsync(sql, new { CountryId = countryID }, string.Join(',', nameof(Character.CharacterId), nameof(Person.PersonId)), SQLWriter.CharacterAndPerson);

            return ResultMessageFactory.CreateOKResult(RetrievalMessage, data);
        }

        public async Task<ResultMessage> UpdateCharacter(CharacterUpdateRequest characterUpdateRequest)
        {
            ResultMessage charRes = await GetCharacter(characterUpdateRequest.CharacterId);

            if (charRes.Data is not Character person)
            {
                charRes.ClearData();
                return charRes;
            }

            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchema(person, characterUpdateRequest);


            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, characterUpdateRequest);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Character), Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Character), Constants.CommonMessages.UpdatePresentAction));
                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }

        }

        public async Task<ResultMessage> DeleteCharacterAndPerson(CharacterDeleteRequest request)
        {
            string characterSQL = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Character.CharacterId));
            string personSQL = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Person.PersonId));

            string sql = string.Join(';', personSQL, characterSQL + ';');
            try
            {
                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Character),
                 Constants.CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Character), Constants.CommonMessages.DeletePresentAction));
                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);

            }
        }
        public async Task<ResultMessage> DeleteCharacter(string characterID)
        {
            string sql = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Character.CharacterId));
            try
            {
                await ExecuteAsync(sql, new { CharacterId = characterID });
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Character),
                 Constants.CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Character), Constants.CommonMessages.DeletePresentAction));
                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);

            }
        }
    }
}