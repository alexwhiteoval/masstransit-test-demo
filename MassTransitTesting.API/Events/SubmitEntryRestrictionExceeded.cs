using MassTransit;

namespace MassTransitTesting.API.Events
{
    public class SubmitEntryRestrictionExceeded : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid[] EntryIds { get; set; } = [];
        public uint? Max { get; private set; }
        public uint CurrentCount { get; private set; }
    }
}
