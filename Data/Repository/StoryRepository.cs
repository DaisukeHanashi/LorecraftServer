using Lorecraft_API.Data.DTO.Story;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Data.Repository
{
    public interface IStoryRepository
    {
        Task<ResultMessage> CreateStory(StoryRequest request);
        Task<ResultMessage> GetStoriesByGenre(int genreID);
        Task<ResultMessage> GetStoriesByLore(long loreID);
        Task<ResultMessage> GetStory(long storyID);
        Task<ResultMessage> UpdateStory(StoryUpdateRequest request);
        Task<ResultMessage> DeleteStory(long storyID);

    }
    public class StoryRepository(SqlConnectionFactory sqlConnectionFactory,
    ILoggerFactory loggerFactory,
    IStoryFactory storyFactory) : BaseRepository(sqlConnectionFactory, loggerFactory), IStoryRepository
    {
        private readonly IStoryFactory _storyFactory = storyFactory;
        public async Task<ResultMessage> CreateStory(StoryRequest request)
        {
            try
            {
                Story story = _storyFactory.Create(request);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(story, story.Properties);
                string success = Constants.CommonMessages.GetSuccessfulActMessage(nameof(Story), Constants.CommonMessages.CreateAction, Single);

                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateCreatedResult(success);
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Story)
                , Constants.CommonMessages.CreatePresentAction));
                _logger.LogError(ex, message);

                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }
        }
        public async Task<ResultMessage> GetStoriesByGenre(int genreID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificParametersForDefaultSchema(nameof(Story), [nameof(Story.GenreId)]).AsNoTracking();

            IEnumerable<Story> data = await GetSomeAsync<Story>(sql, new { GenreId = genreID });
            return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Story),
            Constants.CommonMessages.ReadAction, Plural), data);
        }

        public async Task<ResultMessage> GetStoriesByLore(long loreID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificParametersForDefaultSchema(nameof(Story), [nameof(Story.LoreId)]).AsNoTracking();

            IEnumerable<Story> data = await GetSomeAsync<Story>(sql, new { LoreId = loreID });
            return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Story),
            Constants.CommonMessages.ReadAction, Plural), data);
        }

        public async Task<ResultMessage> GetStory(long storyID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(nameof(Story.StoryId)).AsNoTracking();

            Story? tale = await GetAsync<Story>(sql, new { StoryId = storyID });

            return tale is null ? ResultMessageFactory.CreateNotFoundResult(Constants.CommonMessages.GetNotFoundMessage(nameof(Story)))
            : ResultMessageFactory.CreateOKResult("FOUND!", tale);
        }

        public async Task<ResultMessage> UpdateStory(StoryUpdateRequest request)
        {

            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchemaWithoutEntity(request, nameof(Story));

            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Story),
                Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Story).ToLower(), Constants.CommonMessages.UpdatePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }
        }

        public async Task<ResultMessage> DeleteStory(long storyID)
        {
            string sql = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Story.StoryId));
            try
            {
                await ExecuteAsync(sql, new { StoryId = storyID });
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Story),
                Constants.CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Story).ToLower(), Constants.CommonMessages.DeletePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }
        }
    }
}