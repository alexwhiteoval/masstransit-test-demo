﻿using MassTransit;

namespace MassTransitTesting.API.Events
{
    public class EntryPaymentVerified : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid SubmissionId { get; set; }
        public IEnumerable<Guid>? EntryIds { get; set; }
    }
}
