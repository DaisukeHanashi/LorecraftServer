using Lorecraft_API.Data.DTO.Context;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Manager;

namespace Lorecraft_API.Factory
{
    public interface IContextFactory
    {
        Context Create(ContextRequest request);
    }
    public class ContextFactory : IContextFactory
    {
        public Context Create(ContextRequest request) => new()
        {
            ContextId = GeneratorManager.GenerateStringId(),
            ContextValue = request.ContextValue,
            CharacterId = request.CharacterId,
            CountryId = request.CountryId,
            FactionId = request.FactionId,
            Date = request.Date
        };
    }
}