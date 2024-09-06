using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using MassTransitTesting.API.Entities;
using MassTransitTesting.API.Events;
using Moq;

namespace MassTransitTesting.API.IntegrationTests.Consumers
{
    [Collection( "Shared collection" )]
    public class EntrySubmittedConsumerTests : IntegrationTestBase
    {
        public static DateTime FixedDate => new( 2023, 12, 31, 0, 0, 0, DateTimeKind.Utc );
        private readonly APIFactory _apiFactory;

        public EntrySubmittedConsumerTests( APIFactory apiFactory ) : base( apiFactory )
        {
            _apiFactory = apiFactory;
        }

        [Fact]
        public async Task Consumer_ConsumesMessage()
        {
            // Arrange
            var awardId = NewId.NextGuid();
            var entryId = NewId.NextGuid();

            var entrySubmitted = new EntrySubmitted
            {
                EntryIds = [entryId],
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

        //[Fact]
        public async Task Consumer_GivenCurrentCountExceedsMaxCount_PublishesSubmitEntryRestrictionExceeded()
        {
            // Arrange
            var awardId = NewId.NextGuid();
            var entryId = NewId.NextGuid();
            var entrySubmitRestriction = new EntryRestriction() { Max = 0 };

            _apiFactory.MockEntryRestrictionRepository.Setup( m => m.GetSingle() ).ReturnsAsync( entrySubmitRestriction );

            var entrySubmitted = new EntrySubmitted
            {
                EntryIds = [entryId],
                SubmissionId = NewId.NextGuid(),
                CorrelationId = NewId.NextGuid()
            };

            // Act
            await _harness.Bus.Publish( entrySubmitted );

            // Assert
            ( await _harness.Consumed.Any<EntrySubmitted>( x => x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();

            ( await _harness.Published.Any<SubmitEntryRestrictionExceeded>( x =>
                x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();
        }

        //[Fact]
        public async Task Consumer_GivenCurrentCountDoesNotExceedMaxCount_PublishesCreateAwardActivityEvent()
        {
            // Arrange
            var awardId = NewId.NextGuid();
            var entryId = NewId.NextGuid();
            var entrySubmitRestriction = new EntryRestriction() { Max = 5 };
            var entry = new Entry() { Id = entryId };

            _apiFactory.MockEntryRestrictionRepository.Setup( m => m.GetSingle() ).ReturnsAsync( entrySubmitRestriction );
            _apiFactory.MockEntryRepository.Setup( m => m.FindByIDAsync( It.Is<Guid>( x => x == entryId ), It.IsAny<CancellationToken>() ) ).ReturnsAsync( entry );

            var entrySubmitted = new EntrySubmitted
            {
                EntryIds = [entryId],
                SubmissionId = NewId.NextGuid(),
                CorrelationId = NewId.NextGuid()
            };

            // Act
            await _harness.Bus.Publish( entrySubmitted );

            // Assert
            ( await _harness.Consumed.Any<EntrySubmitted>( x => x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();

            ( await _harness.Published.Any<CreateActivityEvent>( x =>
                x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();
        }

        //[Fact]
        public async Task Consumer_GivenCurrentCountDoesNotExceedMaxCount_PublishesEntrySubmissionsSucceededEvent()
        {
            // Arrange
            var awardId = NewId.NextGuid();
            var entryId = NewId.NextGuid();
            var entrySubmitRestriction = new EntryRestriction() { Max = 5 };
            var entry = new Entry() { Id = entryId };

            _apiFactory.MockEntryRestrictionRepository.Setup( m => m.GetSingle() ).ReturnsAsync( entrySubmitRestriction );
            _apiFactory.MockEntryRepository.Setup( m => m.FindByIDAsync( It.Is<Guid>( x => x == entryId ), It.IsAny<CancellationToken>() ) ).ReturnsAsync( entry );

            var entrySubmitted = new EntrySubmitted
            {
                EntryIds = [entryId],
                SubmissionId = NewId.NextGuid(),
                CorrelationId = NewId.NextGuid()
            };

            // Act
            await _harness.Bus.Publish( entrySubmitted );

            // Assert
            ( await _harness.Consumed.Any<EntrySubmitted>( x => x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();

            ( await _harness.Published.Any<EntrySubmissionsSucceededEvent>( x =>
                x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();
        }

        //[Fact]
        public async Task Consumer_GivenMultipleEntryIdsSubmitted_PublishesMultipleEntriesSubmittedEvent()
        {
            // Arrange
            var awardId = NewId.NextGuid();
            var submissionId = NewId.NextGuid();

            var entryEntrants = NewId.NextGuid( 2 );

            var entryIdsForFirstEntrant = NewId.NextGuid( 10 );
            var entryIdsForAnotherEntrant = NewId.NextGuid( 10 );

            var entrySubmitRestriction = new EntryRestriction() { Max = 1000 };
            var entries = entryIdsForFirstEntrant.Select( x => new Entry() { Id = x, EntrantId = entryEntrants[0], EntrantName = "FirstEntrant", Title = "Title-" + x.ToString() } ).ToList();
            entries.AddRange( entryIdsForAnotherEntrant.Select( x => new Entry() { Id = x, EntrantId = entryEntrants[1], EntrantName = "SecondEntrant", Title = "Title-" + x.ToString() } ).ToList() );

            var entrySubmitted = new EntrySubmitted
            {
                EntryIds = [.. entryIdsForFirstEntrant, .. entryIdsForAnotherEntrant],
                SubmissionId = submissionId,
                CorrelationId = NewId.NextGuid()
            };

            _apiFactory.MockEntryRestrictionRepository.Setup( m => m.GetSingle() ).ReturnsAsync( entrySubmitRestriction );
            _apiFactory.MockEntryRepository.Setup( m => m.FindByAsync( It.IsAny<Guid[]>(), It.IsAny<CancellationToken>() ) ).ReturnsAsync( entries );

            // Act
            await _harness.Bus.Publish( entrySubmitted );

            // Assert
            ( await _harness.Consumed.Any<EntrySubmitted>( x => x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();

            ( await _harness.Published.Any<MultipleEntriesSubmittedEvent>( x =>
                x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();
        }

        //[Fact]
        public async Task Consumer_GivenSingleEntrySubmitted_DoesNotPublishMultipleEntriesSubmittedEvent()
        {
            // Arrange
            var awardId = NewId.NextGuid();
            var submissionId = NewId.NextGuid();
            var entrantId = NewId.NextGuid();
            var entryId = NewId.NextGuid();

            var entrySubmitRestriction = new EntryRestriction() { Max = 1000 };
            var entry = new Entry() { Id = entryId, EntrantId = entrantId, EntrantName = "AnEntrant", Title = "A Title" };

            _apiFactory.MockEntryRestrictionRepository.Setup( m => m.GetSingle() ).ReturnsAsync( entrySubmitRestriction );
            _apiFactory.MockEntryRepository.Setup( m => m.FindByIDAsync( It.Is<Guid>( x => x == entryId ), It.IsAny<CancellationToken>() ) ).ReturnsAsync( entry );

            var entrySubmitted = new EntrySubmitted
            {
                EntryIds = [entryId],
                SubmissionId = submissionId,
                CorrelationId = NewId.NextGuid()
            };

            // Act
            await _harness.Bus.Publish( entrySubmitted );

            // Assert
            // The entry was consumed
            ( await _harness.Consumed.Any<EntrySubmitted>( x => x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeTrue();

            // The entry did not trigger publishing of multiple entry events
            ( await _harness.Published.Any<MultipleEntriesSubmittedEvent>( x =>
                x.Context.Message.CorrelationId == entrySubmitted.CorrelationId ) ).Should().BeFalse();
        }
    }
}
