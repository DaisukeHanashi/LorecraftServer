using Lorecraft_API.Data.DTO.Country;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;

namespace Lorecraft_API.Data.Repository
{
    public interface ICountryRepository
    {
        Task<ResultMessage> GetCountriesByLore(long loreID);
        Task<ResultMessage> CreateCountry(CountryRequest request);
        Task<ResultMessage> UpdateCountry(CountryUpdateRequest request);
        Task<ResultMessage> DeleteCountry(string countryID);
    }
    public class CountryRepository(SqlConnectionFactory sqlConnectionFactory, ILoggerFactory loggerFactory,
    ICountryFactory countryFactory) : BaseRepository(sqlConnectionFactory, loggerFactory), ICountryRepository
    {
        private readonly ICountryFactory _countryFactory = countryFactory;

        public async Task<ResultMessage> GetCountriesByLore(long loreID)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(nameof(Lore.LoreId)).AsNoTracking();

            IEnumerable<Country> countries = await GetSomeAsync<Country>(sql, new { LoreId = loreID });
            return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Country)
            , Constants.CommonMessages.ReadAction, Plural), countries);
        }

        public async Task<ResultMessage> CreateCountry(CountryRequest request)
        {
            try
            {
                Country country = _countryFactory.Create(request);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(country, country.Properties);

                await ExecuteAsync(sql, country);
                return ResultMessageFactory.CreateCreatedResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Account),
                Constants.CommonMessages.CreateAction));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, Constants.CommonMessages.GetFailureMessage(nameof(Account),
                Constants.CommonMessages.CreatePresentAction));

                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);

            }
        }
        public async Task<ResultMessage> UpdateCountry(CountryUpdateRequest request)
        {
            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchemaWithoutEntity(request, nameof(Country));

            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, request);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Country),
                Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string errorMessage = Constants.CommonMessages.GetFailureMessage(nameof(Country).ToLower(), Constants.CommonMessages.UpdatePresentAction);
                _logger.LogError(ex, string.Intern(errorMessage));
                return ResultMessageFactory.CreateInternalServerErrorResult(errorMessage);
            }
        }
        public async Task<ResultMessage> DeleteCountry(string countryID)
        {
            string sql = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(Country.CountryId));

            try
            {
                await ExecuteAsync(sql, new { CountryId = countryID });
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Country), Constants.CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.GetFailureMessage(nameof(Country), Constants.CommonMessages.DeletePresentAction));
            }

        }

    }
}