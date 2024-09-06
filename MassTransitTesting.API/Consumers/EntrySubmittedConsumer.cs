using MassTransit;
using MassTransitTesting.API.Entities;
using MassTransitTesting.API.Events;
using MassTransitTesting.API.Repositories;

namespace MassTransitTesting.API.Consumers
{
    /// <summary>
    /// Consume the EntrySubmitted event
    /// If entry's currentCount reaches to the max count limit,
    /// then it publishes the SubmitEntryRestrictionExceeded event
    /// </summary>
    public class EntrySubmittedConsumer : IConsumer<EntrySubmitted>
    {
        /// <summary>
        /// EntrySubmitRestriction repository
        /// </summary>
        private readonly IEntryRestrictionRepository _entryRestrictionRepository;

        /// <summary>
        /// Entry service
        /// </summary>
        private readonly IEntryRepository _entryRepository;

        /// <summary>
        /// The time provider
        /// </summary>
        private readonly TimeProvider _timeProvider;

        private readonly ILogger<EntrySubmittedConsumer> _logger;
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="entryRestrictionRepository"></param>
        /// <param name="entryRepository"></param>
        /// <param name="timeProvider"></param>
        public EntrySubmittedConsumer( IEntryRestrictionRepository entryRestrictionRepository, IEntryRepository entryRepository, TimeProvider timeProvider, ILogger<EntrySubmittedConsumer> logger )
        {
            _entryRestrictionRepository = entryRestrictionRepository;
            _entryRepository = entryRepository;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        /// <summary>
        /// Consumes the message
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume( ConsumeContext<EntrySubmitted> context )
        {
            _logger.LogInformation( "Cosuming the entry submitted event" );

            // Get the entry-submit-restriction data by awardId
            //var dbEntrySubmitRestriction = await _entryRestrictionRepository.GetSingle();

            //// Increment the currentCount in the EntrySubmitRestriction
            //dbEntrySubmitRestriction.CurrentCount += context.Message.EntryIds.Length;

            //// Persist value in the DB
            //await _entryRestrictionRepository
            //    .ReplaceAsync( dbEntrySubmitRestriction, context.CancellationToken );

            //// Compare current-count with the max - count
            //if( dbEntrySubmitRestriction.Max <= dbEntrySubmitRestriction.CurrentCount )
            //{
            //    // Publish event SubmitEntryRestrictionExceeded
            //    await context.Publish( new SubmitEntryRestrictionExceeded()
            //    {
            //        CorrelationId = context.Message.CorrelationId,
            //        EntryIds = context.Message.EntryIds
            //    } );
            //}
            //else
            //{
            //    if( context.Message.EntryIds.Length == 1 )
            //        await PublishAwardActivityEvent( context );

            //    else
            //        await PublishMultipleEntriesSubmittedEvent( context );

            //    await context.Publish( new EntrySubmissionsSucceededEvent
            //    {
            //        CorrelationId = context.Message.CorrelationId,
            //        EntryIds = context.Message.EntryIds,
            //    } );
            //}
        }

        private async Task PublishAwardActivityEvent( ConsumeContext<EntrySubmitted> context )
        {
            var entry = await _entryRepository.FindByIDAsync( context.Message.EntryIds.First(), context.CancellationToken ) ?? throw new Exception( "No entry found" );

            var createAwardActivityEvent = new CreateActivityEvent
            {
                CorrelationId = context.Message.CorrelationId,
                Type = ActivityType.EntrySubmitted,
                UserId = entry.EntrantId,
                UserName = entry.EntrantName,
                OccurredAt = _timeProvider.GetUtcNow().UtcDateTime,
                Subject = entry.Title,
                SubjectId = entry.Id
            };

            await context.Publish( createAwardActivityEvent, context.CancellationToken );
        }

        private async Task PublishMultipleEntriesSubmittedEvent( ConsumeContext<EntrySubmitted> context )
        {
            var entries = await _entryRepository.FindByAsync( context.Message.EntryIds, context.CancellationToken );

            var entryEntrants = entries.Select( x => new { x.EntrantId, x.EntrantName } ).DistinctBy( x => x.EntrantId );

            if( entries == null || !entries.Any() )
                throw new Exception( "No entries found" );

            var groupedEntries = entries.GroupBy( entry => entry.EntrantId )
                                        .Select( group => new
                                        {
                                            EntrantId = group.Key,
                                            EntryIds = group.Select( x => x.Id )
                                        } )
                                        .ToArray();

            var multipleEntriesSubmittedEvent = new MultipleEntriesSubmittedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                EntrySubmissionId = context.Message.SubmissionId,
                OccurredAt = _timeProvider.GetUtcNow().UtcDateTime,
                EntryEntrantSubmissions = groupedEntries.Select( x => new EntryEntrantSubmission
                {
                    CorrelationId = context.Message.CorrelationId,
                    EntrantId = x.EntrantId,
                    EntrantName = entryEntrants.First( entrant => entrant.EntrantId == x.EntrantId ).EntrantName!,
                    EntryIds = x.EntryIds.ToArray()
                } ).ToArray()
            };

            await context.Publish( multipleEntriesSubmittedEvent, context.CancellationToken );
        }
    }
}
