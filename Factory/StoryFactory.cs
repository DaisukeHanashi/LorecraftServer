using Lorecraft_API.Data.DTO.Story;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Manager;

namespace Lorecraft_API.Factory
{
    public interface IStoryFactory
    {
        Story Create(StoryRequest request);
    }
    public class StoryFactory : IStoryFactory
    {
        public Story Create(StoryRequest request) => new()
        {
            StoryId = GeneratorManager.GenerateStoryId(request.LoreId.ToString()),
            LoreId = request.LoreId,
            Title = request.Title,
            Description = request.Description,
            GenreId = request.GenreId,
            DocLink = request.DocLink
        };
    }
}