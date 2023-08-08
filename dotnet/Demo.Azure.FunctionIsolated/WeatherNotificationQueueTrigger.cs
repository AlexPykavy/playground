using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Demo.Azure.FunctionIsolated
{
    public class WeatherNotificationQueueTrigger
    {
        private readonly ILogger _logger;

        public WeatherNotificationQueueTrigger(ILogger<WeatherNotificationQueueTrigger> logger)
        {
            _logger = logger;
        }

        [Function("WeatherNotificationQueueTrigger")]
        public void Run([QueueTrigger("weather-anomalies")]string message)
        {
            _logger.LogInformation($"Watch out! The weather is inclement: {message}");
        }
    }
}
