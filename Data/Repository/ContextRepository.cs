using Lorecraft_API.Data.DTO.Context;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Data.Repository.Interface;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Data.Repository
{
    public interface IContextRepository
    {
        Task<ResultMessage> CreateContext(ContextRequest request);
        Task<ResultMessage> UpdateContext(ContextUpdateRequest request);
        Task<ResultMessage> GetByCharacter(string characterID);
        Task<ResultMessage> GetByCountry(string countryID);
        Task<ResultMessage> GetByFaction(string factionID);
        Task<ResultMessage> DeleteContext(string contextID);


    }
    public class ContextRepository(SqlConnectionFactory sqlConnectionFactory,
    ILoggerFactory loggerFactory,
    IContextFactory contextFactory) : BaseRepository(sqlConnectionFactory, loggerFactory), IContextRepository
    {
        private readonly IContextFactory _contextFactory = contextFactory;
        public async Task<ResultMessage> CreateContext(ContextRequest request)
        {
            try
            {
                Context context = _contextFactory.Create(request);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(context, context.Properties);
                string success = Constants.CommonMessages.GetSuccessfulActMessage(nameof(Context), Constants.CommonMessages.CreateAction, Single);

                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateCreatedResult(success);
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Context)
                , Constants.CommonMessages.CreatePresentAction));
                _logger.LogError(ex, message);

                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }
        }
        public async Task<ResultMessage> UpdateContext(ContextUpdateRequest request)
        {
            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchemaWithoutEntity(request, nameof(Context));

            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Context),
                Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Context).ToLower(), Constants.CommonMessages.UpdatePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }
        }
        public async Task<ResultMessage> GetByCharacter(string characterID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificParametersForDefaultSchema(nameof(Context), [nameof(Context.CharacterId)]).AsNoTracking();

            IEnumerable<Context> data = await GetSomeAsync<Context>(sql, new { CharacterId = characterID });
            return data is null ? ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Context)))
            : ResultMessageFactory.CreateOKResult("FOUND!", data);
        }
        public async Task<ResultMessage> GetByCountry(string countryID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificParametersForDefaultSchema(nameof(Context), [nameof(Context.CountryId)]).AsNoTracking();

            IEnumerable<Context> data = await GetSomeAsync<Context>(sql, new { CountryId = countryID });
            return data is null ? ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Context)))
            : ResultMessageFactory.CreateOKResult("FOUND!", data);
        }
        public async Task<ResultMessage> GetByFaction(string factionID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificParametersForDefaultSchema(nameof(Context), [nameof(Context.FactionId)]).AsNoTracking();

            IEnumerable<Context> data = await GetSomeAsync<Context>(sql, new { FactionId = factionID });
            return data is null ? ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Context)))
            : ResultMessageFactory.CreateOKResult("FOUND!", data);
        }

        public async Task<ResultMessage> DeleteContext(string contextID)
        {
            string sql = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Context.ContextId));
            try
            {
                await ExecuteAsync(sql, new { ContextId = contextID });
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Context),
                Constants.CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Context).ToLower(), Constants.CommonMessages.DeletePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }
        }

    }
}