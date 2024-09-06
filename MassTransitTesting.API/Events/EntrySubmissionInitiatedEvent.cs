using MassTransit;

namespace MassTransitTesting.API.Events
{
    public class EntrySubmissionInitiatedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
