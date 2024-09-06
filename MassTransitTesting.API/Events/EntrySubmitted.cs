﻿using MassTransit;

namespace MassTransitTesting.API.Events
{
    public class EntrySubmitted : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
