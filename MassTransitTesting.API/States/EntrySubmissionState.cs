using MassTransit;
using MassTransitTesting.API.Entities;

namespace MassTransitTesting.API.States
{
    public class EntrySubmissionState :
        SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string? CurrentState { get; set; }
        public int Version { get; set; }
        public Guid SubmissionId { get; set; }
        public IEnumerable<EntryCategoryPair>? Items { get; set; }
    }
}
