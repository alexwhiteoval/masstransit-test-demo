using MassTransit;
using MassTransitTesting.API.Entities;

namespace MassTransitTesting.API.Events
{
    public class CreateActivityEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public ActivityType Type { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid? SubjectId { get; set; }
        public string? Subject { get; set; }
    }
}
