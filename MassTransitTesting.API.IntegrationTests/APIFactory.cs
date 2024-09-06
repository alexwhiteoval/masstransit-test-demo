using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace MassTransitTesting.API.IntegrationTests
{
    public class APIFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        protected override void ConfigureWebHost( IWebHostBuilder builder )
        {
            builder.UseEnvironment( "Testing" );
            base.ConfigureWebHost( builder );

            builder.ConfigureTestServices( services =>
            {
                services.AddMassTransitTestHarness( x =>
                {
                    x.AddDelayedMessageScheduler();
                    x.UsingInMemory( ( context, cfg ) =>
                    {
                        cfg.ConfigureEndpoints( context );
                    } );
                } );
            } );
        }

        public Task InitializeAsync()
        {
            HttpClient = CreateClient();
            return Task.CompletedTask;
        }

        Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;

        public HttpClient HttpClient { get; set; } = default!;
    }
}
