using Eveneum;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    internal interface IStreamRepository<T> where T : AggregateRoot, new()
    {
        Task<StreamHeaderResponse> GetHeaderAsync(string streamId);
        Task<T?> GetAsync(string streamId);
        Task SaveAsync(T entity, object? metadata = null, string? streamId = null);
    }
}