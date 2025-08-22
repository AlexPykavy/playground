using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;
using Azure.Data.SchemaRegistry;
using MassTransit;
using Demo.Azure.Messaging.Model;
using Demo.Azure.Messaging.Options;

namespace Demo.Azure.Messaging.Controllers;

[ApiController]
[Route("[controller]")]
public class SchemaController : ControllerBase
{
    private readonly ILogger<SchemaController> _logger;
    private readonly SchemaRegistryOptions _schemaRegistryOptions;
    private readonly SchemaRegistryClient _schemaRegistryClient;

    public SchemaController(
        ILogger<SchemaController> logger,
        IOptions<SchemaRegistryOptions> schemaRegistryOptions,
        SchemaRegistryClient schemaRegistryClient)
    {
        _logger = logger;
        _schemaRegistryOptions = schemaRegistryOptions.Value;
        _schemaRegistryClient = schemaRegistryClient;
    }

    [HttpPut]
    public async Task<IActionResult> Put()
    {
        await _schemaRegistryClient.RegisterSchemaAsync(
            _schemaRegistryOptions.GroupName,
            Message.SchemaName,
            Message.SchemaDefinition,
            SchemaFormat.Avro);

        return Accepted();
    }
}
