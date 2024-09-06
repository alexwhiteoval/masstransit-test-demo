using MassTransitTesting.API.Entities;

namespace MassTransitTesting.API.Repositories
{
    public interface IEntryRepository
    {
        Task<Entry> FindByIDAsync( Guid id, CancellationToken cancellationToken );
        Task<IEnumerable<Entry>> FindByAsync( Guid[] ids, CancellationToken cancellationToken );
    }
}
