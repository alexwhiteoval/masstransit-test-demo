using MassTransitTesting.API.Entities;

namespace MassTransitTesting.API.Repositories
{
    public interface IEntryRestrictionRepository
    {
        Task<EntryRestriction> GetSingle();
        Task ReplaceAsync( EntryRestriction entryRestriction, CancellationToken cancellationToken );
    }
}
