using Lorecraft_API.Data.DTO.Lore;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory.Interface;
using Lorecraft_API.Manager;

namespace Lorecraft_API.Factory
{
    public class LoreFactory : ILoreFactory
    {
        public Lore Create(LoreRequest req) => new(){
            LoreId = GeneratorManager.GenerateLoreId(req.AccountId.ToString() ?? "0"),
            AccountId = req.AccountId ?? 0, 
            UniverseName = req.UniverseName, 
            IsPublic = req.IsPublic
        };
    }
}