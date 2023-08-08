using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo.Azure.FunctionIsolated
{
    public class WeatherNotificationTimerTrigger
    {
        private readonly ILogger _logger;

        public WeatherNotificationTimerTrigger(ILogger<WeatherNotificationTimerTrigger> logger)
        {
            _logger = logger;
        }

        [Function("WeatherNotificationTimerTrigger")]
        public void Run([TimerTrigger("0 */2 * * * *")] TimerInfo timer, string message)
        {
            _logger.LogInformation(DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
        }
    }
}
