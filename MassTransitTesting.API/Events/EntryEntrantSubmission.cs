using MassTransit;

namespace MassTransitTesting.API.Events
{
    public class EntryEntrantSubmission : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid EntrantId { get; set; }
        public string EntrantName { get; set; } = string.Empty;
        public Guid[] EntryIds { get; set; } = [];
    }
}
