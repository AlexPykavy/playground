using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo.Azure.FunctionIsolated
{
    public class WeatherNotificationBlobTrigger
    {
        private readonly ILogger _logger;

        public WeatherNotificationBlobTrigger(ILogger<WeatherNotificationBlobTrigger> logger)
        {
            _logger = logger;
        }

        [Function("WeatherNotificationBlobTrigger")]
        [BlobOutput("weather-forecast-output/{name}")]
        public string Run(
            [BlobTrigger("weather-forecast-input/{name}")] string blob,
            [BlobInput("weather-forecast-templates/common.txt")] string template)
        {
            _logger.LogInformation("Generaring weather forecast for {blob} using the template {template}", blob, template);

            return string.Format(template, blob);
        }
    }
}
