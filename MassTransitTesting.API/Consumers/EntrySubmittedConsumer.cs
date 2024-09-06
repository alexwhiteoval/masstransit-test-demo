using MassTransit;
using MassTransitTesting.API.Events;

namespace MassTransitTesting.API.Consumers
{
    public class EntrySubmittedConsumer : IConsumer<EntrySubmitted>
    {
        private readonly ILogger<EntrySubmittedConsumer> _logger;
        
        public EntrySubmittedConsumer( ILogger<EntrySubmittedConsumer> logger )
        {
            _logger = logger;
        }

        public async Task Consume( ConsumeContext<EntrySubmitted> context )
        {
            _logger.LogInformation( "Cosuming the entry submitted event" );
            await Task.CompletedTask;
        }
    }
}
