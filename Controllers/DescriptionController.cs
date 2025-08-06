using Lorecraft_API.Data.DTO.Description;
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
    public class DescriptionController(IDescriptionRepository descriptionRepository) : ControllerBase
    {
        private readonly IDescriptionRepository _descriptionRepository = descriptionRepository;
        [HttpGet("get/{conceptID}")]
        [Authorize]
        public async Task<IActionResult> GetDescriptionsByConcept([FromRoute] string conceptID)
        {
            var result = await _descriptionRepository.GetDescriptions(conceptID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPost("create")]
        [Authorize]
        [Consumes(ContentType.ApplicationJson)]
        public async Task<IActionResult> CreateDescription([FromBody] DescriptionRequest request)
        {
            var result = await _descriptionRepository.CreateDescription(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPatch("update")]
        [Authorize]
        [Consumes(ContentType.ApplicationJson)]
        public async Task<IActionResult> UpdateDescription([FromBody] DescriptionUpdateRequest request)
        {
            var result = await _descriptionRepository.UpdateDescription(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpDelete("delete/{descriptionID}")]
        [Authorize]
        public async Task<IActionResult> DeleteDescription([FromRoute] string descriptionID)
        {
            var result = await _descriptionRepository.DeleteDescription(descriptionID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
    }
}