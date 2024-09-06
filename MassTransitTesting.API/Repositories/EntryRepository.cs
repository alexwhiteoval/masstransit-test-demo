using MassTransitTesting.API.Entities;

namespace MassTransitTesting.API.Repositories
{
    public class EntryRepository : IEntryRepository
    {
        public Task<IEnumerable<Entry>> FindByAsync( Guid[] ids, CancellationToken cancellationToken ) => Task.FromResult( ids.Select( x => new Entry() { Id = x, EntrantId = x, EntrantName = x.ToString(), Title = x.ToString() } ) );
        public Task<Entry> FindByIDAsync( Guid id, CancellationToken cancellationToken ) => Task.FromResult( new Entry() { Id = id, EntrantId = id, EntrantName = id.ToString(), Title = id.ToString() } );
    }
}
