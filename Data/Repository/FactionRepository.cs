using Lorecraft_API.Data.DTO.Faction;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Data.Repository
{
    public interface IFactionRepository
    {
        Task<ResultMessage> GetByLore(long loreID);
        Task<ResultMessage> CreateFaction(FactionRequest request);
        Task<ResultMessage> UpdateFaction(FactionUpdateRequest request);
        Task<ResultMessage> DeleteFaction(string factionID);

    }
    public class FactionRepository(SqlConnectionFactory sqlConnectionFactory,
    ILoggerFactory loggerFactory, IFactionFactory factionFactory) : BaseRepository(sqlConnectionFactory, loggerFactory), IFactionRepository
    {
        private readonly IFactionFactory _factionFactory = factionFactory;
        public async Task<ResultMessage> GetByLore(long loreID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificParametersForDefaultSchema(nameof(Lore), [nameof(Lore.LoreId)]).AsNoTracking();

            IEnumerable<Faction> data = await GetSomeAsync<Faction>(sql, new { LoreId = loreID });
            return data is null ? ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Lore)))
        : ResultMessageFactory.CreateOKResult("FOUND!", data);
        }

        public async Task<ResultMessage> CreateFaction(FactionRequest request)
        {
            try
            {
                Faction faction = _factionFactory.Create(request);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(faction, faction.Properties);
                string success = Constants.CommonMessages.GetSuccessfulActMessage(nameof(Faction), Constants.CommonMessages.CreateAction, Single);

                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateCreatedResult(success);
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Faction)
                , Constants.CommonMessages.CreatePresentAction));
                _logger.LogError(ex, message);

                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }
        }
        public async Task<ResultMessage> UpdateFaction(FactionUpdateRequest request)
        {
            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchemaWithoutEntity(request, nameof(Faction));

            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Faction),
                Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Faction).ToLower(), Constants.CommonMessages.UpdatePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }
        }
        public async Task<ResultMessage> DeleteFaction(string factionID)
        {
            string sql = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Faction.FactionId));
            try
            {
                await ExecuteAsync(sql, new { FactionId = factionID });
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Faction),
                Constants.CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Faction).ToLower(), Constants.CommonMessages.DeletePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }
        }
    }
}