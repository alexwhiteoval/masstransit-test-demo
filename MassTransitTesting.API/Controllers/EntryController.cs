using MassTransit;
using MassTransitTesting.API.Entities;
using MassTransitTesting.API.Events;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MassTransitTesting.API.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EntryController( IPublishEndpoint publishEndpoint ) 
        { 
            _publishEndpoint = publishEndpoint;
        }

        // POST api/<EntryController>
        [HttpPost]
        public async Task<OkObjectResult> Post( [FromBody] EntryCategoryPair[] items )
        {
            var id = NewId.NextGuid();
            await _publishEndpoint.Publish( new EntrySubmissionInitiatedEvent
            {
                CorrelationId = id,
                SubmissionId = id,
                Items = items
            } );
            return Ok( new { submissionId = id } );
        }

        [HttpPost( "submit" )]
        public async Task<OkObjectResult> Submit( [FromBody] EntryCategoryPair[] items )
        {
            var id = NewId.NextGuid();
            await _publishEndpoint.Publish( new EntrySubmitted
            {
                CorrelationId = id,
                SubmissionId = id,
                EntryIds = items.Select( x => x.EntryId ).ToArray()
            } );
            return Ok( new { submissionId = id } );
        }
    }
}
