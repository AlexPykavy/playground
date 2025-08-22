using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Azure.Data.SchemaRegistry;
using MassTransit;
using Demo.Azure.Messaging;
using Demo.Azure.Messaging.Model;
using Demo.Azure.Messaging.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection("AzureAd"));
builder.Services.Configure<EventHubOptions>(builder.Configuration.GetSection("EventHub"));
builder.Services.Configure<SchemaRegistryOptions>(builder.Configuration.GetSection("SchemaRegistry"));
builder.Services.Configure<ServiceBusOptions>(builder.Configuration.GetSection("ServiceBus"));

builder.Services.AddMassTransit(x =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    var azureAdOptions = serviceProvider.GetService<IOptions<AzureAdOptions>>()!.Value;
    var serviceBusOptions = serviceProvider.GetService<IOptions<ServiceBusOptions>>()!.Value;

    x.UsingAzureServiceBus((context, cfg) =>
    {
        if (string.IsNullOrEmpty(serviceBusOptions.ConnectionString))
        {
            cfg.Host(new Uri(serviceBusOptions.Url), c => {
                c.TokenCredential = new ClientSecretCredential(
                    azureAdOptions.TenantId,
                    azureAdOptions.ClientId,
                    azureAdOptions.ClientSecret);
            });
        }
        else
        {
            cfg.Host(serviceBusOptions.ConnectionString);
        }

        cfg.Message<Message>(x =>
        {
            x.SetEntityName(serviceBusOptions.TopicName);
        });
    });
});

builder.Services.AddMassTransit<IEventHubBus>(x =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    var eventHubOptions = serviceProvider.GetService<IOptions<EventHubOptions>>()!.Value;

    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(eventHubOptions.ConnectionString);
    });

    x.AddRider(rider =>
    {
        rider.UsingEventHub((context, k) =>
        {
            k.Host(eventHubOptions.ConnectionString);
        });
    });
});

builder.Services.AddScoped<SchemaRegistryClient>(serviceProvider =>
{
    var azureAdOptions = serviceProvider.GetService<IOptions<AzureAdOptions>>()!.Value;
    var schemaRegistryOptions = serviceProvider.GetService<IOptions<SchemaRegistryOptions>>()!.Value;

    return new SchemaRegistryClient(
        schemaRegistryOptions.Namespace,
        new ClientSecretCredential(azureAdOptions.TenantId, azureAdOptions.ClientId, azureAdOptions.ClientSecret));
});

builder.Services.AddScoped<EventHubProducerClient>(serviceProvider =>
{
    var azureAdOptions = serviceProvider.GetService<IOptions<AzureAdOptions>>()!.Value;
    var eventHubOptions = serviceProvider.GetService<IOptions<EventHubOptions>>()!.Value;

    var producerOptions = new EventHubProducerClientOptions
    {
        ConnectionOptions = new EventHubConnectionOptions
        {
            TransportType = EventHubsTransportType.AmqpTcp
        }
    };

    var credential = new ClientSecretCredential(
        azureAdOptions.TenantId,
        azureAdOptions.ClientId,
        azureAdOptions.ClientSecret);

    return new EventHubProducerClient(
            new Uri(eventHubOptions.Url).Host,
            eventHubOptions.HubName,
            credential,
            producerOptions);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
