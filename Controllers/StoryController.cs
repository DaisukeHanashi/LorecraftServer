using Lorecraft_API.Data.DTO.Story;
using Lorecraft_API.Data.Repository;
using Lorecraft_API.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lorecraft_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Cors.EnableCors(Constants.CorsSetting.AllowLocal)]
    public class StoryController(IStoryRepository storyRepository) : ControllerBase
    {
        private readonly IStoryRepository _storyRepository = storyRepository;

        [HttpGet("get/byGenre/{genreID:int}")]
        [Authorize]
        public async Task<IActionResult> GetByGenre([FromRoute] int genreID)
        {
            var result = await _storyRepository.GetStoriesByGenre(genreID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpGet("get/byLore/{loreID:long}")]
        [Authorize]
        public async Task<IActionResult> GetByLore([FromRoute] long loreID)
        {
            var result = await _storyRepository.GetStoriesByLore(loreID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPost("create")]
        [Authorize]
        [Consumes(Constants.ContentType.ApplicationJson)]
        public async Task<IActionResult> CreateStory([FromBody] StoryRequest request)
        {
            var result = await _storyRepository.CreateStory(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPatch("update")]
        [Authorize]
        [Consumes(Constants.ContentType.ApplicationJson)]
        public async Task<IActionResult> UpdateStory([FromBody] StoryUpdateRequest request)
        {
            var result = await _storyRepository.UpdateStory(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpDelete("delete/{storyID:long}")]
        [Authorize]
        public async Task<IActionResult> DeleteStory([FromRoute] long storyID)
        {
            var result = await _storyRepository.DeleteStory(storyID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }

        
    }
}