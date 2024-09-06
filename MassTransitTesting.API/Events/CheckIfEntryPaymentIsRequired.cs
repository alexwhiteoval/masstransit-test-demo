using MassTransit;
using MassTransitTesting.API.Entities;

namespace MassTransitTesting.API.Events
{
    public class CheckIfEntryPaymentIsRequired : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public Guid SubmissionId { get; init; }
        public IEnumerable<EntryCategoryPair>? Items { get; set; }
    }
}
