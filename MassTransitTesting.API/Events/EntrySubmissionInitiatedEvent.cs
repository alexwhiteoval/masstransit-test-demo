using MassTransit;
using MassTransitTesting.API.Entities;

namespace MassTransitTesting.API.Events
{
    public class EntrySubmissionInitiatedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid SubmissionId { get; set; }
        public IEnumerable<EntryCategoryPair>? Items { get; set; }
    }
}
