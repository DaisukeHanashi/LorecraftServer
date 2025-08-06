using Lorecraft_API.Data.DTO.Character;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Data.Repository
{
    public interface IPersonRespository
    {
        Task<ResultMessage> CreatePerson(CharacterRequest request);
        Task<ResultMessage> UpdatePerson(CharacterUpdateRequest request);
    }
    public class PersonRepository(SqlConnectionFactory sqlConnectionFactory
    , ILoggerFactory loggerFactory
    , ICharacterFactory characterFactory)
    : BaseRepository(sqlConnectionFactory, loggerFactory), IPersonRespository
    {
        private readonly ICharacterFactory _characterFactory = characterFactory;
        public async Task<ResultMessage> CreatePerson(CharacterRequest request)
        {
            try
            {
                Person person = _characterFactory.CreatePerson(request);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(person, person.Properties);
                await ExecuteAsync(sql, person);
                return ResultMessageFactory.CreateCreatedResult("Person successfully created!", person.PersonId);

            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, "Failed to create a person");
                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }

        }
        private async Task<ResultMessage> GetPerson(string personID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(personID).AsNoTracking();

            Person? datum = await GetAsync<Person>(sql, new { PersonId = personID });

            return datum is null ? ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Person)))
          : ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Person), Constants.CommonMessages.ReadAction), datum);

        }
        public async Task<ResultMessage> UpdatePerson(CharacterUpdateRequest request)
        {
            if (string.IsNullOrEmpty(request.PersonId))
                return ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.ArgumentNotFound);


            ResultMessage perRes = await GetPerson(request.PersonId);

            if (perRes.Data is not Person person)
            {
                perRes.ClearData();
                return perRes;
            }

            PersonUpdateRequest? perRequest = _characterFactory.MapPersonUpdate(request);

            if (perRequest is null)
                return ResultMessageFactory.CreateBadRequestResult(Constants.CommonMessages.IDNotFound);

            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchema(person, perRequest);

            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, perRequest);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Person), Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Person), Constants.CommonMessages.UpdatePresentAction));
                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }

        }
    }
}