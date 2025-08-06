using Lorecraft_API.Data.DTO.Faction;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Manager;

namespace Lorecraft_API.Factory
{
    public interface IFactionFactory
    {
        Faction Create(FactionRequest request);
    }
    public class FactionFactory : IFactionFactory
    {
        public Faction Create(FactionRequest request) => new()
        {
            FactionId = GeneratorManager.GenerateFactionId(request.LoreId.ToString()),
            LoreId = request.LoreId,
            Name = request.Name,
            Type = request.Type,
            Purpose = request.Purpose
        };
    }
}