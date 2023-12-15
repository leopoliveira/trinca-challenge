using System.Threading.Tasks;

using Domain.Repositories;

namespace Domain.Services.Generic
{
    public class BaseInterface<T> : IBaseInterface<T> where T : AggregateRoot, new()
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

        public virtual async Task SaveAsync(T entity)
        {
            await _repository.SaveAsync(entity);
        }
    }
}
