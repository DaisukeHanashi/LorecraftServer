using Lorecraft_API.Data.DTO.Context;
using Lorecraft_API.Data.Repository;
using Lorecraft_API.Data.Repository.Interface;
using Lorecraft_API.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lorecraft_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Cors.EnableCors(Constants.CorsSetting.AllowLocal)]
    public class ContextController(IContextRepository contextRepository) : ControllerBase
    {
        private readonly IContextRepository _contextRepository = contextRepository;

        [HttpGet("get/byCharacter/{characterID}")]
        [Authorize]
        public async Task<IActionResult> GetByCharacter([FromRoute] string characterID)
        {
            var result = await _contextRepository.GetByCharacter(characterID);

            return result.Code == StatusCodes.Status200OK ? Ok(result) : Conflict(result);
        }
        [HttpGet("get/byCountry/{countryID}")]
        [Authorize]
        public async Task<IActionResult> GetByCountry([FromRoute] string countryID)
        {
            var result = await _contextRepository.GetByCountry(countryID);

            return result.Code == StatusCodes.Status200OK ? Ok(result) : Conflict(result);
        }
        [HttpGet("get/byFaction/{factionID}")]
        [Authorize]
        public async Task<IActionResult> GetByFaction([FromRoute] string factionID)
        {
            var result = await _contextRepository.GetByFaction(factionID);

            return result.Code == StatusCodes.Status200OK ? Ok(result) : Conflict(result);
        }
        [HttpPost("create")]
        [Authorize]
        [Consumes(Constants.ContentType.ApplicationJson)]
        public async Task<IActionResult> CreateContext([FromBody] ContextRequest contextRequest)
        {
            var result = await _contextRepository.CreateContext(contextRequest);

            return result.Code == StatusCodes.Status201Created ? Ok(result) : Conflict(result);
        }
        [HttpPatch("update")]
        [Authorize]
        [Consumes(Constants.ContentType.ApplicationJson)]
        public async Task<IActionResult> UpdateContext([FromBody] ContextUpdateRequest request)
        {
            var result = await _contextRepository.UpdateContext(request);

            return result.Code == StatusCodes.Status200OK ? Ok(result) : Conflict(result);
        }
        [HttpPatch("delete/{contextID}")]
        [Authorize]
        public async Task<IActionResult> UpdateContext([FromBody] string contextID)
        {
            var result = await _contextRepository.DeleteContext(contextID);

            return result.Code == StatusCodes.Status200OK ? Ok(result) : Conflict(result);
        }


    }
}