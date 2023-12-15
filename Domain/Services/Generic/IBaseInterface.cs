using System.Threading.Tasks;

namespace Domain.Services.Generic
{
    public interface IBaseInterface<T> where T : AggregateRoot
    {
        Task<T> GetAsync(string id);
        Task SaveAsync(T entity);
    }
}
