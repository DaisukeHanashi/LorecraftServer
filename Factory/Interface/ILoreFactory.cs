using Lorecraft_API.Data.DTO.Lore;
using Lorecraft_API.Data.Models;

namespace Lorecraft_API.Factory.Interface
{
    public interface ILoreFactory
    {
        Lore Create(LoreRequest req); 
    }
}