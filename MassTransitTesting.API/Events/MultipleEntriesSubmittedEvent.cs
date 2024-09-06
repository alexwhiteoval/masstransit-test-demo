using MassTransit;

namespace MassTransitTesting.API.Events
{
    public class MultipleEntriesSubmittedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid EntrySubmissionId { get; set; }
        public DateTime OccurredAt { get; set; }
        public EntryEntrantSubmission[] EntryEntrantSubmissions { get; set; } = [];
    }
}
