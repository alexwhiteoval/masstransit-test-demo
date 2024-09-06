using MassTransit;
using MassTransitTesting.API.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

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
                services.RemoveAll<IEntryRepository>();
                services.RemoveAll<IEntryRestrictionRepository>();
                services.AddScoped( _ => MockEntryRepository.Object );
                services.AddScoped( _ => MockEntryRestrictionRepository.Object );

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
        public Mock<IEntryRepository> MockEntryRepository { get; } = new Mock<IEntryRepository>();
        public Mock<IEntryRestrictionRepository> MockEntryRestrictionRepository { get; } = new Mock<IEntryRestrictionRepository>();
    }
}
