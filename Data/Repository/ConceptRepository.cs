using Lorecraft_API.Data.DTO.Concept;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Data.Repository
{
    public interface IConceptRepository
    {
        Task<ResultMessage> Create(ConceptRequest req);
        Task<ResultMessage> GetLoreConcepts(long loreID);
        Task<ResultMessage> UpdateConcept(ConceptRequest req);
        Task<ResultMessage> DeleteConcept(string conceptID);
    }

    public class ConceptRepository(SqlConnectionFactory sqlConnectionFactory,
    ILoggerFactory loggerFactory,
    IConceptFactory conceptFactory,
    IDescriptionRepository descriptionRepository) : BaseRepository(sqlConnectionFactory, loggerFactory), IConceptRepository
    {
        private readonly IConceptFactory _conceptFactory = conceptFactory;
        private readonly IDescriptionRepository _descriptionRepository = descriptionRepository;
        public async Task<ResultMessage> Create(ConceptRequest req)
        {
            try
            {
                Concept concept = _conceptFactory.Create(req);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(concept, concept.Properties);
                string success = Constants.CommonMessages.GetSuccessfulActMessage(nameof(Concept), Constants.CommonMessages.CreateAction, Single);

                await ExecuteAsync(sql, req);
                return ResultMessageFactory.CreateCreatedResult(success);
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Concept)
                , Constants.CommonMessages.CreatePresentAction));
                _logger.LogError(ex, message);

                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }
        }
        public async Task<ResultMessage> GetLoreConcepts(long loreID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificParametersForDefaultSchema(nameof(Concept), [nameof(Concept.LoreId)]).AsNoTracking();

            IEnumerable<Concept> data = await GetSomeAsync<Concept>(sql, new { LoreId = loreID });

            return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Concept),
            Constants.CommonMessages.ReadAction, Plural), data);
        }
        public async Task<ResultMessage> UpdateConcept(ConceptRequest req)
        {
            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchemaWithoutEntity(req, nameof(Concept));

            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, req);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Concept),
                Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Concept).ToLower(), Constants.CommonMessages.UpdatePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }

        }
        public async Task<ResultMessage> DeleteConcept(string conceptID)
        {
            var descriptionResult = await _descriptionRepository.GetDescriptions(conceptID); 

            if(descriptionResult.Data is not IEnumerable<Description> relatedDescriptions)
             return descriptionResult; 

            string[] descriptionIdentifiers = _conceptFactory.ExtractIDsFromDescriptionData(relatedDescriptions);

            string descriptionSQL = descriptionIdentifiers.Length > 0 ? SQLWriter.GenerateDeleteSqlWithMultipleIdentitiesForDefaultSchema(nameof(Description.DescriptionId), descriptionIdentifiers)
            : string.Empty;
            string conceptSQL = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Concept.ConceptId));

            string sql = !string.IsNullOrEmpty(descriptionSQL) ? string.Join(';', descriptionSQL, conceptSQL + ';') : conceptSQL;

            try
            {
                await ExecuteAsync(sql, new { ConceptId = conceptID });
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Concept), Constants.CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.GetFailureMessage(nameof(Concept), Constants.CommonMessages.DeletePresentAction));
            }

        }

    }
}