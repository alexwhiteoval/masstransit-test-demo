namespace MassTransitTesting.API.IntegrationTests
{
    [CollectionDefinition( "Shared collection", DisableParallelization = true )]
    public class SharedTestCollection : ICollectionFixture<APIFactory>
    {
    }
}
