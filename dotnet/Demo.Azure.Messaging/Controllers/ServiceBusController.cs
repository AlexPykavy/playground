using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;
using MassTransit;
using Demo.Azure.Messaging.Model;
using Demo.Azure.Messaging.Options;

namespace Demo.Azure.Messaging.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceBusController : ControllerBase
{
    private readonly ILogger<ServiceBusController> _logger;
    private readonly IBus _bus;

    public ServiceBusController(
        ILogger<ServiceBusController> logger,
        IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    [HttpPut]
    public async Task<IActionResult> Put()
    {
        await _bus.Publish(new Message { Text = $"The time is {DateTimeOffset.Now}" });

        _logger.LogInformation("Published a message to Service Bus using MassTransit");

        return Accepted();
    }
}
