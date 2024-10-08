﻿using MassTransit.Testing;

namespace MassTransitTesting.API.IntegrationTests
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected readonly HttpClient _client;
        protected readonly ITestHarness _harness;

        public IntegrationTestBase( APIFactory factory )
        {
            _client = factory.HttpClient;
            _harness = factory.Services.GetTestHarness();
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
