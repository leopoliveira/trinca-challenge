using System.Threading.Tasks;

using Domain.Entities;

namespace Domain.Services
{
    public interface ILookupService
    {
        Task<Lookups> GetLookups();
    }
}
