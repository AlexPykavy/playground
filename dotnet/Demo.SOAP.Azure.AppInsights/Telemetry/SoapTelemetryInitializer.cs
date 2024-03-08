using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;

namespace Demo.SOAP.Azure.AppInsights.Telemetry;

public class SoapTelemetryInitializer : ITelemetryInitializer
{
    public async void Initialize(ITelemetry telemetry)
    {
        var dependency = telemetry as DependencyTelemetry;

        if (dependency == null)
        {
            return;
        }

        if (dependency.Context.TryGetRawObject("HttpRequest", out var request) && request is HttpRequestMessage httpRequest)
        {
            ProcessHttpRequest(httpRequest, dependency);
        }

        if (dependency.Context.TryGetRawObject("HttpResponse", out var response) && response is HttpResponseMessage httpResponse)
        {
            await ProcessHttpResponse(httpResponse, dependency);
        }
    }

    private void ProcessHttpRequest(HttpRequestMessage httpRequest, DependencyTelemetry dependency)
    {
        if (httpRequest.Headers.TryGetValues("SOAPAction", out var values) && values.Any())
        {
            dependency.Context.GlobalProperties["SOAPAction"] = values.First().Trim('"');
        }
    }

    private async Task ProcessHttpResponse(HttpResponseMessage httpResponse, DependencyTelemetry dependency)
    {
        if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
        {
            var responseBody = await httpResponse.Content.ReadAsStringAsync();

            if (responseBody.Contains("not found", StringComparison.InvariantCulture)
                || responseBody.Contains("No data found", StringComparison.InvariantCulture))
            {
                dependency.ResultCode = HttpStatusCode.NotFound.ToString("d");
            }
        }
    }
}
