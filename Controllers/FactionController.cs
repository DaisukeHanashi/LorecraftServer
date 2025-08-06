using Lorecraft_API.Data.DTO.Faction;
using Lorecraft_API.Data.Repository;
using Lorecraft_API.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Lorecraft_API.Resources.Constants;

namespace Lorecraft_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Cors.EnableCors(CorsSetting.AllowLocal)]
    public class FactionController(IFactionRepository factionRepository) : ControllerBase
    {
        private readonly IFactionRepository _factionRepository = factionRepository;

        [HttpGet("get/{loreID:long}")]
        [Authorize]
        public async Task<IActionResult> GetFaction([FromRoute] long loreID)
        {
            var result = await _factionRepository.GetByLore(loreID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPost("create")]
        [Authorize]
        [Consumes(ContentType.ApplicationJson)]
        public async Task<IActionResult> CreateFaction([FromBody] FactionRequest request)
        {
            var result = await _factionRepository.CreateFaction(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPatch("update")]
        [Authorize]
        [Consumes(ContentType.ApplicationJson)]
        public async Task<IActionResult> UpdateFaction([FromBody] FactionUpdateRequest request)
        {
            var result = await _factionRepository.UpdateFaction(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpDelete("delete/{factionID}")]
        [Authorize]
        public async Task<IActionResult> DeleteFaction([FromRoute] string factionID)
        {
            var result = await _factionRepository.DeleteFaction(factionID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        
    }
}