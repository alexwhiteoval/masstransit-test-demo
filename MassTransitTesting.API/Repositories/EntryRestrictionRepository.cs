using MassTransitTesting.API.Entities;

namespace MassTransitTesting.API.Repositories
{
    public class EntryRestrictionRepository : IEntryRestrictionRepository
    {
        public Task<EntryRestriction> GetSingle() => Task.FromResult( new EntryRestriction() { CurrentCount = 0, Max = 5 } );
        public Task ReplaceAsync( EntryRestriction entryRestriction, CancellationToken cancellationToken ) => Task.CompletedTask;
    }
}
