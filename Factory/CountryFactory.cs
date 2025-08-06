using Lorecraft_API.Data.DTO.Country;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Manager;

namespace Lorecraft_API.Factory
{
    public interface ICountryFactory
    {
        Country Create(CountryRequest request);
    }
    public class CountryFactory : ICountryFactory
    {
        public Country Create(CountryRequest request) => new()
        {
            CountryId = GeneratorManager.GenerateStringId(request.LoreId.ToString()),
            LoreId = request.LoreId,
            Name = request.Name,
            Continent = request.Continent,
            GovernmentType = request.GovernmentType
        };
    }
}