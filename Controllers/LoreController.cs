using Lorecraft_API.Data.DTO.Lore;
using Lorecraft_API.Data.Repository;
using Lorecraft_API.Manager;
using Lorecraft_API.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lorecraft_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Cors.EnableCors(Constants.CorsSetting.AllowLocal)]
    public class LoreController(ILoreRepository loreRepository,
    AuthenticationManager authManager) : ControllerBase
    {
        private readonly ILoreRepository _loreRepository = loreRepository;
        private readonly AuthenticationManager _authManager = authManager;

        [HttpGet("get/worlds")]
        [Authorize]
        public async Task<IActionResult> GetWorlds([FromHeader(Name = Constants.Identifier)] string identifier)
        {
            if (!_authManager.CheckAndGetUserID(identifier, out long accID))
                return BadRequest(Constants.ValueUnknown);

            var result = await _loreRepository.GetLores(accID);

            return result.IsStatusCodeAllOK() ? Ok(result.Data) : Conflict(result);

        }

        [HttpGet("get/publicWorlds")]
        [Authorize]
        public async Task<IActionResult> GetPublicWorlds()
        {
            var result = await _loreRepository.GetPublicLores();

            return result.IsStatusCodeAllOK() ? Ok(result.Data) : Conflict(result);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateWorld([FromBody] LoreRequest req, [FromHeader(Name = Constants.Identifier)] string identifier)
        {
            if (!_authManager.CheckAndGetUserID(identifier, out long accID))
                return BadRequest(Constants.ValueUnknown);

            req.AccountId = accID;

            var result = await _loreRepository.CreateLore(req);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPatch("edit")]
        [Authorize]
        public async Task<IActionResult> EditWorld([FromBody] LoreRequest req)
        {
            var result = await _loreRepository.UpdateLore(req);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteWorld([FromBody] LoreDeleteRequest req)
        {
            var result = await _loreRepository.DeleteLore(req.LoreId);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);

        }

    }
}