using System.Threading.Tasks;

using Domain.Repositories;

namespace Domain.Services.Generic
{
    internal class BaseInterface<T> : IBaseInterface<T> where T : AggregateRoot, new()
    {
        private readonly IStreamRepository<T> _repository;

        public BaseInterface(IStreamRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<T> GetAsync(string id)
        {
            return await _repository.GetAsync(id);
        }

        public virtual async Task SaveAsync(T entity, object? metadata = null, string? streamId = null)
        {
            await _repository.SaveAsync(entity, metadata, streamId);
        }
    }
}
