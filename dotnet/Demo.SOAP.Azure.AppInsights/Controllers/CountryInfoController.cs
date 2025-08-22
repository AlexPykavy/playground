using Microsoft.AspNetCore.Mvc;
using CountryInfoServiceReference;

namespace Demo.SOAP.Azure.AppInsights.Controllers;

[ApiController]
[Route("[controller]")]
public class CountryInfoController : ControllerBase
{
    private readonly ILogger<CountryInfoController> _logger;
    private readonly CountryInfoServiceSoapTypeClient _countryInfoServiceSoapClient;

    public CountryInfoController(ILogger<CountryInfoController> logger)
    {
        _logger = logger;
        _countryInfoServiceSoapClient = new CountryInfoServiceSoapTypeClient(CountryInfoServiceSoapTypeClient.EndpointConfiguration.CountryInfoServiceSoap12);
    }

    [HttpGet(Name = "ListOfContinentsByNameAsync")]
    public async Task<tContinent[]> ListOfContinentsByNameAsync()
    {
        try
        {
            var response = await _countryInfoServiceSoapClient.ListOfContinentsByNameAsync();

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred: {0}", ex);

            return null;
        }
    }
}
