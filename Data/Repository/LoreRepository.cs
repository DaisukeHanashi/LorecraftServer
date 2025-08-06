using Lorecraft_API.Data.DTO.Lore;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory.Interface;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;


namespace Lorecraft_API.Data.Repository
{
    public interface ILoreRepository
    {
        Task<ResultMessage> CreateLore(LoreRequest req); 
        Task<ResultMessage> GetLores(long accountID);
        Task<ResultMessage> GetPublicLores();
        Task<ResultMessage> UpdateLore(LoreRequest req);
        Task<ResultMessage> DeleteLore(long loreID); 
    }
    public class LoreRepository(SqlConnectionFactory factory,
    ILoggerFactory loggerFactory,
    ILoreFactory loreFactory) : BaseRepository(factory, loggerFactory), ILoreRepository
    {
        private readonly ILoreFactory _loreFactory = loreFactory;
        private readonly bool IsPublic = true; 

        public async Task<ResultMessage> CreateLore(LoreRequest req)
        {
            try
            {
                Lore lore = _loreFactory.Create(req);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(nameof(Lore), lore.Properties);
                
                await ExecuteAsync(sql, lore);
                return ResultMessageFactory.CreateCreatedResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Lore), Constants.CommonMessages.CreateAction
                ));
            }

            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Lore), Constants.CommonMessages.CreatePresentAction));

                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }

        }

        public async Task<ResultMessage> GetLores(long accountID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(nameof(Lore.AccountId)).AsNoTracking();

            IEnumerable<Lore> lores = await GetSomeAsync<Lore>(sql, new { AccountId = accountID });

            return ResultMessageFactory.CreateOKResult("Thy worlds hath been received!", lores);
        }

        public async Task<ResultMessage> GetLore(long loreId)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(nameof(Lore.LoreId)).AsNoTracking();

            Lore? lore = await GetAsync<Lore>(sql, new { LoreId = loreId });

            return lore is null ? ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Lore)))
            : ResultMessageFactory.CreateOKResult("FOUND!", lore);

        }

        public async Task<ResultMessage> GetPublicLores()
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificParametersForDefaultSchema(nameof(Lore), [nameof(Lore.IsPublic)]).AsNoTracking();

            IEnumerable<Lore> lores = await GetSomeAsync<Lore>(sql, new { IsPublic });

            return ResultMessageFactory.CreateOKResult("Thy worlds hath been received!", lores);

        }

        public async Task<ResultMessage> UpdateLore(LoreRequest req)
        {
            if(req.LoreId is null)
              return ResultMessageFactory.CreateBadRequestResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Lore)));

            ResultMessage loreResult = await GetLore((long)req.LoreId);

            if (loreResult.Data is not Lore lore)
                return loreResult;

            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchema(lore, req);

            if(string.IsNullOrEmpty(sql))
              return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, req);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(
                    nameof(Lore), Constants.CommonMessages.UpdateAction
                ));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(
                    nameof(Lore), Constants.CommonMessages.UpdatePresentAction
                ));

                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }
        }


        public async Task<ResultMessage> DeleteLore(long loreID)
        {
            string sql = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Lore.LoreId));

            try
            {
                await ExecuteAsync(sql, new { LoreId = loreID });
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(
                    nameof(Lore), Constants.CommonMessages.DeleteAction
                ));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(
                    nameof(Lore), Constants.CommonMessages.DeletePresentAction
                ));

                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }


        }
    }
}