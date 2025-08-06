using Lorecraft_API.Data.DTO.Character;
using Lorecraft_API.Resources;
using Lorecraft_API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Lorecraft_API.Resources.Constants; 

namespace Lorecraft_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Cors.EnableCors(CorsSetting.AllowLocal)]

    public class CharacterController(ICharacterService characterService) : ControllerBase
    {
        private readonly ICharacterService _characterService = characterService;

        [HttpGet("get/characterOnly/{characterID}")]
        [Authorize]
        public async Task<IActionResult> GetCharacterOnly([FromRoute] string characterID)
        {
            var result = await _characterService.GetCharacters(characterID, CharacterRepositoryMode.CharacterOnly);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpGet("get/byLore/{loreID:long}")]
        [Authorize]
        public async Task<IActionResult> GetCharactersByLore([FromRoute] long loreID)
        {
            var result = await _characterService.GetCharacters(loreID, CharacterRepositoryMode.CharacterLore);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpGet("get/byFaction/{factionID}")]
        [Authorize]
        public async Task<IActionResult> GetCharactersByFaction([FromRoute] string factionID)
        {
            var result = await _characterService.GetCharacters(factionID, CharacterRepositoryMode.CharacterFaction);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpGet("get/byCountry/{countryID}")]
        [Authorize]
        public async Task<IActionResult> GetCharactersByCountry([FromRoute] string countryID)
        {
            var result = await _characterService.GetCharacters(countryID, CharacterRepositoryMode.CharacterCountry);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateCharacter([FromBody] CharacterRequest request)
        {
            var result = await _characterService.CreateCharacter(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> EditCharacter([FromBody] CharacterUpdateRequest request)
        {
            var result = await _characterService.UpdateCharacter(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpDelete("delete/{characterID}")]
        [Authorize]
        public async Task<IActionResult> DeleteCharacter([FromRoute] string characterID)
        {
            var result = await _characterService.DeleteCharacterOnce(characterID);
            
            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        
    }
}