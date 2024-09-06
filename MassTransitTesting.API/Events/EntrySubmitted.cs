using MassTransit;

namespace MassTransitTesting.API.Events
{
    public class EntrySubmitted : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid SubmissionId { get; set; }
        public Guid[] EntryIds { get; set; } = [];
    }
}
