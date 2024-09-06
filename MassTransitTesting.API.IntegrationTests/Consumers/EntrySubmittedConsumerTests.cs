using FluentAssertions;
using MassTransit;
using MassTransitTesting.API.Events;

namespace MassTransitTesting.API.IntegrationTests.Consumers
{
    [Collection( "Shared collection" )]
    public class EntrySubmittedConsumerTests : IntegrationTestBase
    {
        public EntrySubmittedConsumerTests( APIFactory apiFactory ) : base( apiFactory )
        {
        }

        [Fact]
        public async Task Consumer_ConsumesMessage()
        {
            var entrySubmitted = new EntrySubmitted
            {
                CorrelationId = NewId.NextGuid()
            };

            await _harness.Bus.Publish( entrySubmitted );


            var result = await _harness.Consumed.Any<EntrySubmitted>( x => x.Context.Message.CorrelationId == entrySubmitted.CorrelationId );
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Consumer_IgnoresEvent()
        {
            var entryEvent = new EntrySubmissionInitiatedEvent()
            {
                CorrelationId = NewId.NextGuid()
            };

            await _harness.Bus.Publish( entryEvent );

            var result = await _harness.Consumed.Any<EntrySubmitted>( x => x.Context.Message.CorrelationId == entryEvent.CorrelationId );
            result.Should().BeFalse();
        }
    }
}
