using Lorecraft_API.Data.DTO.Description;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Data.Repository
{
    public interface IDescriptionRepository
    {
        Task<ResultMessage> GetDescriptions(string conceptID);
        Task<ResultMessage> CreateDescription(DescriptionRequest request);
        Task<ResultMessage> UpdateDescription(DescriptionUpdateRequest request);
        Task<ResultMessage> DeleteDescription(string descriptionID);
    }
    public class DescriptionRepository(SqlConnectionFactory sqlConnectionFactory,
    ILoggerFactory loggerFactory, IDescriptionFactory descriptionFactory) : BaseRepository(sqlConnectionFactory, loggerFactory), IDescriptionRepository
    {
        private readonly IDescriptionFactory _descriptionFactory = descriptionFactory;

        public async Task<ResultMessage> GetDescriptions(string conceptID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSortByForDefaultSchema(nameof(Description), [nameof(Concept.ConceptId)], [nameof(Description.DescriptionId)], Constants.HasOrderNumberOnSQL, [nameof(Description.OrderN)], Constants.SortingMode.Ascending);

            IEnumerable<Description> descriptions = await GetSomeAsync<Description>(sql, new { ConceptId = conceptID });

            return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Description), Constants.CommonMessages.ReadAction, Plural), descriptions);
        }
        public async Task<ResultMessage> CreateDescription(DescriptionRequest request)
        {
            try
            {
                Description description = _descriptionFactory.Create(request);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(description, description.Properties);

                await ExecuteAsync(sql, description);
                return ResultMessageFactory.CreateCreatedResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Description),
                 Constants.CommonMessages.CreateAction));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Description), Constants.CommonMessages.CreatePresentAction));
                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }
        }
        public async Task<ResultMessage> UpdateDescription(DescriptionUpdateRequest request)
        {
            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchemaWithoutEntity(request, nameof(Description));

            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Description),
                Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Description).ToLower(), Constants.CommonMessages.UpdatePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }
        }
        public async Task<ResultMessage> DeleteDescription(string descriptionID)
        {
            string sql = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Description.DescriptionId));

            try
            {
                await ExecuteAsync(sql, new { DescriptionId = descriptionID });
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Description), Constants.CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.GetFailureMessage(nameof(Description), Constants.CommonMessages.DeletePresentAction));
            }

        }
    }
}