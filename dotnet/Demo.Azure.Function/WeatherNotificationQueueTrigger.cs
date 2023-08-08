using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Demo.Azure.Function
{
    public class WeatherNotificationQueueTrigger
    {
        [FunctionName("WeatherNotificationQueueTrigger")]
        public void Run([QueueTrigger("weather-anomalies")]string message, ILogger log)
        {
            log.LogInformation($"Watch out! The weather is inclement: {message}");
        }
    }
}
