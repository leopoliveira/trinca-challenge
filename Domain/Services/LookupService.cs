using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CrossCutting;

using Domain.Entities;

namespace Domain.Services
{
    internal class LookupService : ILookupService
    {
        private readonly SnapshotStore _snapshots;

        private const string LOOKUPS_COLLECTION_NAME = "Lookups";

        public LookupService(SnapshotStore snapshots)
        {
            _snapshots = snapshots;
        }

        public async Task<Lookups> GetLookups()
        {
            return await _snapshots
                        .AsQueryable<Lookups>(LOOKUPS_COLLECTION_NAME)
                        .SingleOrDefaultAsync();
        }

        public async Task<List<string>> GetModeratorsId()
        {
            return await _snapshots
                        .AsQueryable<Lookups>(LOOKUPS_COLLECTION_NAME)
                        .Select(l => l.ModeratorIds)
                        .SingleOrDefaultAsync();
        }
    }
}
