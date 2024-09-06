using MassTransit;

namespace MassTransitTesting.API.Events
{
    public class EntrySubmissionsSucceededEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid[] EntryIds { get; set; } = [];
    }
}
