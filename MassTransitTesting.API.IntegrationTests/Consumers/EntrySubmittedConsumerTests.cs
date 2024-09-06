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
            // Arrange
            var entrySubmitted = new EntrySubmitted
            {
                EntryIds = [NewId.NextGuid()],
                SubmissionId = NewId.NextGuid(),
                CorrelationId = NewId.NextGuid()
            };

            // Act
            await _harness.Bus.Publish( entrySubmitted );

            // Assert
            ( await _harness.Consumed.Any<EntrySubmitted>( x =>
                x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();
        }

        [Fact]
        public async Task Consumer_IgnoresEvent()
        {
            var entryEvent = new EntrySubmissionInitiatedEvent()
            {
                CorrelationId = NewId.NextGuid()
            };

            await _harness.Bus.Publish( entryEvent );

            ( await _harness.Consumed.Any<EntrySubmitted>( x => x.Context.Message.CorrelationId == entryEvent.CorrelationId ) ).Should().BeFalse();
        }
    }
}
