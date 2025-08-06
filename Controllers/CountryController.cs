using Lorecraft_API.Data.DTO.Country;
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
    public class CountryController(ICountryRepository countryRepository) : ControllerBase
    {
        private readonly ICountryRepository _countryRepository = countryRepository;
        [HttpGet("get/{loreID:long}")]
        [Authorize]
        public async Task<IActionResult> GetByLore([FromRoute] long loreID)
        {
            var result = await _countryRepository.GetCountriesByLore(loreID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPost("create")]
        [Authorize]
        [Consumes(ContentType.ApplicationJson)]
        public async Task<IActionResult> CreateCountry([FromBody] CountryRequest request)
        {
            var result = await _countryRepository.CreateCountry(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpPatch("update")]
        [Consumes(ContentType.ApplicationJson)]
        [Authorize]
        public async Task<IActionResult> UpdateCountry([FromBody] CountryUpdateRequest request)
        {
            var result = await _countryRepository.UpdateCountry(request);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }
        [HttpDelete("delete/{countryID}")]
        [Authorize]
        public async Task<IActionResult> DeleteCountry([FromRoute] string countryID)
        {
            var result = await _countryRepository.DeleteCountry(countryID);

            return result.IsStatusCodeAllOK() ? Ok(result) : Conflict(result);
        }

    }
}