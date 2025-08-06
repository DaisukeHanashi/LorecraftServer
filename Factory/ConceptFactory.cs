using Lorecraft_API.Data.DTO.Concept;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Manager;

namespace Lorecraft_API.Factory
{
    public interface IConceptFactory
    {
        Concept Create(ConceptRequest req);
        string[] ExtractIDsFromDescriptionData(IEnumerable<Description> descriptions);
    }
    public class ConceptFactory : IConceptFactory
    {
        public Concept Create(ConceptRequest req) => new()
        {
            ConceptId = GeneratorManager.GenerateStringId(),
            LoreId = req.LoreId,
            Type = req.Type,
            Name = req.Name
        };
        public string[] ExtractIDsFromDescriptionData(IEnumerable<Description> descriptions) => [.. descriptions.Select(desc => desc.DescriptionId)];
    }
}