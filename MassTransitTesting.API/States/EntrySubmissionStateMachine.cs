using MassTransit;
using MassTransitTesting.API.Events;

namespace MassTransitTesting.API.States
{
    public class EntrySubmissionStateMachine
        : MassTransitStateMachine<EntrySubmissionState>
    {
        /// <summary>
        /// A payment response is required
        /// </summary>
        public State? AwaitingPaymentResponse { get; private set; }

        /// <summary>
        /// A payment is not required
        /// </summary>
        public State? PaymentNotRequired { get; private set; }

        /// <summary>
        /// A payment is received
        /// </summary>
        public State? PaymentReceived { get; private set; }

        /// <summary>
        /// A entry submitted and state completed
        /// </summary>
        public State? Completed { get; private set; }

        /// <summary>
        /// Fired upon the initial user api request
        /// </summary>
        public Event<EntrySubmissionInitiatedEvent>? Requested { get; private set; }

        /// <summary>
        /// Fired when the payment has been checked and not applicable
        /// </summary>
        public Event<EntryPaymentNotApplicable>? PaymentNotApplicable { get; private set; }

        /// <summary>
        /// Fired when the payment has been verified
        /// </summary>
        public Event<EntryPaymentVerified>? PaymentVerified { get; private set; }

        /// <summary>
        /// Fired when the entry has been submitted
        /// </summary>
        public Event<EntrySubmitted>? EntrySubmitted { get; private set; }


        public EntrySubmissionStateMachine()
        {
            // Configure the state property
            InstanceState( x => x.CurrentState );

            // When the state machine is in Initial state
            // Set the instance with the request and
            // Publish the check if entry payment is required command
            // Set the state to AwaitingPaymentResponse
            Initially(
                When( Requested )
                    .Then(
                        context =>
                        {
                            context.Saga.CorrelationId = context.Message.CorrelationId;
                            context.Saga.SubmissionId = context.Message.SubmissionId;
                            context.Saga.Items = context.Message.Items;
                        }
                    )
                    .TransitionTo( AwaitingPaymentResponse )
                    .PublishAsync( context =>
                        context.Init<CheckIfEntryPaymentIsRequired>( new CheckIfEntryPaymentIsRequired
                        {
                            CorrelationId = context.Saga.CorrelationId,
                            SubmissionId = context.Saga.SubmissionId,
                            Items = context.Saga.Items,
                        } ) ) );

            // When the state machine is in AwaitingPayment state and
            // When the Payment Verified event is fired
            // Publish the EntryReadyToSubmit event
            // Set the state to PaymentNotRequired
            During(
                AwaitingPaymentResponse,
                When( PaymentNotApplicable )
                    .TransitionTo( PaymentNotRequired )
                    .PublishAsync(
                        context => context.Init<EntriesReadyToSubmit>( new
                        {
                            context.Saga.CorrelationId,
                            context.Saga.SubmissionId
                        } ) ) );

            // When the state machine is in AwaitingPayment state and
            // When the Payment Verified event is fired
            // Publish the EntryReadyToSubmit event
            // Set the state to PaymentReceived
            During(
                AwaitingPaymentResponse,
                When( PaymentVerified )
                    .TransitionTo( PaymentReceived )
                    .PublishAsync(
                        context => context.Init<EntriesReadyToSubmit>( new
                        {
                            context.Saga.CorrelationId,
                            context.Saga.SubmissionId
                        } ) ) );

            // When the state machine is in PaymentReceived or PaymentNotReceived state
            // When the EntrySubmitted event is fired
            // Transition to Completed
            During(
                PaymentReceived,
                PaymentNotRequired,
                When( EntrySubmitted )
                    .TransitionTo( Completed )
                    .Finalize() );

            SetCompletedWhenFinalized();
        }
    }
}
