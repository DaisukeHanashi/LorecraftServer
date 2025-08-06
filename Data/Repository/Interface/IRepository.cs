using Lorecraft_API.Resources;

namespace Lorecraft_API.Data.Repository.Interface
{
    public interface IRepository
    {
        Task<ResultMessage> Create(object request);
        Task<ResultMessage> Update(object request, string? mode = null);
        Task<ResultMessage> Get(object? request, string? mode = null);
        Task<ResultMessage> Delete(object request, string? mode = null);

    }
}