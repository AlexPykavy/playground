using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;
using MassTransit;
using MassTransit.DependencyInjection;
using System.Text;
using Demo.Azure.Messaging.Model;
using Demo.Azure.Messaging.Options;

namespace Demo.Azure.Messaging.Controllers;

[ApiController]
[Route("[controller]")]
public class EventHubController : ControllerBase
{
    private readonly ILogger<EventHubController> _logger;
    private readonly IEventHubProducerProvider _producerProvider;
    private readonly EventHubOptions _eventHubOptions;
    private readonly SchemaRegistryOptions _schemaRegistryOptions;
    private readonly SchemaRegistryClient _schemaRegistryClient;
    private readonly EventHubProducerClient _producerClient;

    public EventHubController(
        ILogger<EventHubController> logger,
        Bind<IEventHubBus, IEventHubProducerProvider> producerProvider,
        IOptions<EventHubOptions> eventHubOptions,
        IOptions<SchemaRegistryOptions> schemaRegistryOptions,
        SchemaRegistryClient schemaRegistryClient,
        EventHubProducerClient producerClient)
    {
        _logger = logger;
        _producerProvider = producerProvider.Value;
        _eventHubOptions = eventHubOptions.Value;
        _schemaRegistryOptions = schemaRegistryOptions.Value;
        _schemaRegistryClient = schemaRegistryClient;
        _producerClient = producerClient;
    }

    [HttpPut]
    public async Task<IActionResult> Put()
    {
        var eventHubProducer = await _producerProvider.GetProducer(_eventHubOptions.HubName);
        await eventHubProducer.Produce(new Message { Text = $"The time is {DateTimeOffset.Now}" });

        _logger.LogInformation("Produced an event to Event Hub using MassTransit");

        return Accepted();
    }

    [HttpPut("UsingStandardSDK")]
    public async Task<IActionResult> PutUsingStandardSDK()
    {
        var serializer = new SchemaRegistryAvroSerializer(_schemaRegistryClient, _schemaRegistryOptions.GroupName, new SchemaRegistryAvroSerializerOptions { AutoRegisterSchemas = true });

        await _producerClient.SendAsync(new[] { (EventData)await serializer.SerializeAsync(new Message { Text = $"The time is {DateTimeOffset.Now}" }, messageType: typeof(EventData)) });

        _logger.LogInformation("Produced an event to Event Hub using SDK");

        return Accepted();
    }
}
