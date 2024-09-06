using MassTransit;
using MassTransitTesting.API.Repositories;

var builder = WebApplication.CreateBuilder( args );

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IEntryRepository, EntryRepository>();
builder.Services.AddScoped<IEntryRestrictionRepository, EntryRestrictionRepository>();
builder.Services.AddSingleton( TimeProvider.System );

builder.Services.AddMassTransit( x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.SetInMemorySagaRepositoryProvider();

    x.AddActivities( typeof( Program ).Assembly );
    x.AddConsumers( typeof( Program ).Assembly );
    x.AddSagaStateMachines( typeof( Program ).Assembly );
    x.AddSagas( typeof( Program ).Assembly );

    x.SetEndpointNameFormatter( new KebabCaseEndpointNameFormatter( includeNamespace: true ) );
    x.UsingInMemory( (context, cfg) =>
    {
        cfg.ConfigureEndpoints( context );
    } );
} );
builder.Services.AddOptions<MassTransitHostOptions>().Configure( options => options.WaitUntilStarted = true );

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }