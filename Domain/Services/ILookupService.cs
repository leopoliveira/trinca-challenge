using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.Entities;

namespace Domain.Services
{
    public interface ILookupService
    {
        Task<Lookups> GetLookups();
        Task<List<string>> GetModeratorsId();
    }
}
