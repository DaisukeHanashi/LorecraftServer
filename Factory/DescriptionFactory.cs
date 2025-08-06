using Lorecraft_API.Data.DTO.Description;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Manager;

namespace Lorecraft_API.Factory
{
    public interface IDescriptionFactory
    {
        Description Create(DescriptionRequest request);
    }
    public class DescriptionFactory : IDescriptionFactory
    {
        public Description Create(DescriptionRequest request) => new()
        {
            DescriptionId = GeneratorManager.GenerateStringId(request.ConceptId),
            ConceptId = request.ConceptId,
            Type = request.Type,
            Content = request.Content,
            OrderN = request.OrderN
        };
    }
}