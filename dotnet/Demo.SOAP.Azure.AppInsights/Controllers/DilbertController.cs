using Microsoft.AspNetCore.Mvc;
using ServiceReference;

namespace Demo.SOAP.Azure.AppInsights.Controllers;

[ApiController]
[Route("[controller]")]
public class DilbertController : ControllerBase
{
    private readonly ILogger<DilbertController> _logger;
    private readonly DilbertSoapClient _dilbertSoapClient;

    public DilbertController(ILogger<DilbertController> logger)
    {
        _logger = logger;
        _dilbertSoapClient = new DilbertSoapClient(DilbertSoapClient.EndpointConfiguration.DilbertSoap);
    }

    [HttpGet(Name = "TodaysDilbertAsync")]
    public async Task<string> TodaysDilbertAsync()
    {
        try
        {
            var response = await _dilbertSoapClient.TodaysDilbertAsync();

            return response.Body.TodaysDilbertResult;
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred: {0}", ex);

            return null;
        }
    }
}
