using Lorecraft_API.Data.DTO.Concept;
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
    public class ConceptController(IConceptRepository conceptRepository) : ControllerBase
    {
        private readonly IConceptRepository _conceptRepository = conceptRepository;

        [HttpPost("create")]
        [Authorize]
        [Consumes(Constants.ContentType.ApplicationJson)]
        public async Task<IActionResult> CreateConcept([FromBody] ConceptRequest request)
        {
            var result = await _conceptRepository.Create(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpGet("get/{loreID:long}")]
        [Authorize]
        public async Task<IActionResult> GetConcept([FromRoute] long loreID)
        {
            var result = await _conceptRepository.GetLoreConcepts(loreID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPatch("update")]
        [Consumes(Constants.ContentType.ApplicationJson)]
        [Authorize]
        public async Task<IActionResult> UpdateConcept([FromBody] ConceptRequest request)
        {
            var result = await _conceptRepository.UpdateConcept(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpDelete("delete/{conceptID}")]
        [Authorize]
        public async Task<IActionResult> DeleteConcept([FromRoute] string conceptID)
        {
            var result = await _conceptRepository.DeleteConcept(conceptID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }



    }
}