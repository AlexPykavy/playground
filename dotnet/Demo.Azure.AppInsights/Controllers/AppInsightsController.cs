using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Azure;
using Azure.Identity;
using Azure.Core;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Demo.Azure.AppInsights.Options;

namespace Demo.Azure.AppInsights.Controllers;

[ApiController]
[Route("[controller]")]
public class AppInsightsController : ControllerBase
{
    private readonly ILogger<AppInsightsController> _logger;
    private readonly ApplicationInsightsOptions _appInsightsOptions;
    private readonly AzureAdOptions _azureAdOptions;

    public AppInsightsController(
        ILogger<AppInsightsController> logger,
        IOptions<ApplicationInsightsOptions> appInsightsOptions,
        IOptions<AzureAdOptions> azureAdOptions)
    {
        _logger = logger;
        _appInsightsOptions = appInsightsOptions.Value;
        _azureAdOptions = azureAdOptions.Value;
    }

    [HttpGet(Name = "GetRequests")]
    public async IAsyncEnumerable<string> Get()
    {
        var credential = new ClientSecretCredential(
            _azureAdOptions.TenantId,
            _azureAdOptions.ClientId,
            _azureAdOptions.ClientSecret);
        var client = new LogsQueryClient(credential);

        Response<LogsQueryResult> result = await client.QueryResourceAsync(
            new ResourceIdentifier(_appInsightsOptions.ResourceId),
            "requests | top 10 by timestamp",
            new QueryTimeRange(TimeSpan.FromDays(1)));
        LogsTable table = result.Value.Table;

        foreach (var row in table.Rows)
        {
            yield return $"{row["timestamp"]} {row["name"]} {row["duration"]}";
        }
    }
}
